using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

// SkillEffect ���� ��-���׸� ������
public class SkillEffectConverter : JsonConverter
{
    // SkillEffect �� �� ���� Ÿ�Ե鿡�� ����ǵ���
    public override bool CanConvert(Type objectType) =>
        typeof(SkillEffect).IsAssignableFrom(objectType);

    // Read ����
    public override bool CanRead => true;
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        var jo = JObject.Load(reader);

        // 1) discriminator �б�
        var typeStr = jo["effectType"]?.Value<string>();
        if (string.IsNullOrEmpty(typeStr))
            throw new JsonSerializationException("effects �׸� 'type' �ʵ尡 �����ϴ�.");

        // 2) ���� �ʵ� �Ľ�
        var statType = jo["statType"]?.ToObject<CharacterStatType?>(serializer);
        var value = jo["value"]?.ToObject<float?>(serializer) ?? 0f;

        // 3) ��ü Ÿ�� �б�
        switch (typeStr.ToLowerInvariant())
        {
            case "damage":
                var damage = jo["damage"]?.ToObject<Damage>(serializer);
                return new DamageEffect(statType, value, damage);

            case "statMul":
                return new StatMulEffect(statType, value);

            case "statPlus":
                return new StatMulEffect(statType, value);

            default:
                throw new NotSupportedException($"�������� �ʴ� effect type: {typeStr}");
        }
    }

    // Write ���� (�ʿ� ������ Serialize �ʸ� ����)
    public override bool CanWrite => true;
    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

}