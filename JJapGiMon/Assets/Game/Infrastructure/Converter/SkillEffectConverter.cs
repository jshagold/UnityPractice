using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

// SkillEffect 전용 비-제네릭 컨버터
public class SkillEffectConverter : JsonConverter
{
    // SkillEffect 와 그 하위 타입들에만 적용되도록
    public override bool CanConvert(Type objectType) =>
        typeof(SkillEffect).IsAssignableFrom(objectType);

    // Read 지원
    public override bool CanRead => true;
    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        var jo = JObject.Load(reader);

        // 1) discriminator 읽기
        var typeStr = jo["effectType"]?.Value<string>();
        if (string.IsNullOrEmpty(typeStr))
            throw new JsonSerializationException("effects 항목에 'type' 필드가 없습니다.");

        // 2) 공통 필드 파싱
        var statType = jo["statType"]?.ToObject<CharacterStatType?>(serializer);
        var value = jo["value"]?.ToObject<float?>(serializer) ?? 0f;

        // 3) 구체 타입 분기
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
                throw new NotSupportedException($"지원하지 않는 effect type: {typeStr}");
        }
    }

    // Write 지원 (필요 없으면 Serialize 쪽만 위임)
    public override bool CanWrite => true;
    public override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }

}