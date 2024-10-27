﻿using System.Globalization;
using Vogen.Tests.Types;

namespace ConsumerTests.ToStringTests;

public class BasicFunctionality
{
    [Fact]
    public void Defaults_to_empty_string_if_primitive_returns_null()
    {
        VoWrappingNaughtyPrimitive v = VoWrappingNaughtyPrimitive.From(new NaughtyPrimitive());
        v.ToString().Should().Be(string.Empty);
    }

    [Fact]
    public void Does_not_use_ToString_from_record()
    {
        MyRecordVo record = MyRecordVo.From(123);
        record.ToString().Should().Be("123");
    }

    [Fact]
    public void Generates_correct_nullability_for_when_vo_record_derives_from_a_record()
    {
        MyDerivedRecordVo record = MyDerivedRecordVo.From(123);
        record.ToString().Should().Be("123");
    }
    
    [SkippableIfBuiltWithNoValidationFlagFact]
    public void ToString_does_not_throw_for_something_uninitialized()
    {
#pragma warning disable VOG010
        Age age = new Age();
        age.ToString().Should().Be("[UNINITIALIZED]");
#pragma warning restore VOG010
    }

    [SkippableIfNotBuiltWithNoValidationFlagFact]
    public void ToString_does_not_show_uninitialized_when_no_validation_is_on()
    {
#pragma warning disable VOG010
        Age age = new Age();
        age.ToString().Should().Be("0");
#pragma warning restore VOG010
    }

    [SkippableIfNotBuiltWithNoValidationFlagFact]
    public void ToString_with_format_does_not_show_uninitialized_when_no_validation_is_on()
    {
#pragma warning disable VOG010
        Age age = new Age();
        age.ToString("b8").Should().Be("00000000");
#pragma warning restore VOG010
    }

    [Fact]
    public void With_collections_it_uses_the_underlying_types_ToString() => FileHash.From(new Hash<byte>([1, 2, 3])).ToString().Should().Be("Vogen.Tests.Types.Hash`1[System.Byte]");

    [Fact]
    public void ToString_uses_generated_method()
    {
        Age.From(18).ToString().Should().Be("18");
        Age.From(100).ToString().Should().Be("100");
        Age.From(1_000).ToString().Should().Be("1000");

        Name.From("fred").ToString().Should().Be("fred");
        Name.From("barney").ToString().Should().Be("barney");
        Name.From("wilma").ToString().Should().Be("wilma");
    }

    [Fact]
    public void ToString_with_format_uses_IFormattable_methods()
    {
        Age.From(18).ToString().Should().Be("18");
        Age.From(100).ToString("x8").Should().Be("00000064");
        
        
        Age.From((int)Math.Pow(2, 8)).ToString("E").Should().Be("2.560000E+002");
        
        Name.From("fred").ToString().Should().Be("fred");
        Name.From("barney").ToString().Should().Be("barney");
        Name.From("wilma").ToString().Should().Be("wilma");
    }

    [Fact]
    public void TryFormat_delegates_to_primitive()
    {
        Span<char> s = stackalloc char[8];
        Age.From(128).TryFormat(s, out int written, "x8", CultureInfo.InvariantCulture).Should().BeTrue();
        written.Should().Be(8);
        s.ToString().Should().Be("00000080");

        MyDecimal d = MyDecimal.From(1.23m);

        $"{d:0.000}".Should().Be("1.230");
        d.ToString("0.00", new CultureInfo("fr")).Should().Be("1,23");
        $"{d:0.000}".Should().Be("1.230");

        Span<char> s2 = stackalloc char[8];
        MyDecimal.From(1.23m).TryFormat(s2, out written, "000.00", CultureInfo.InvariantCulture).Should().BeTrue();
        written.Should().Be(6);
        s2[..written].ToString().Should().Be("001.23");
    }
}

[ValueObject<NaughtyPrimitive>]
public partial class VoWrappingNaughtyPrimitive
{
}

public class NaughtyPrimitive
{
    public override string? ToString() => null;
}

[ValueObject]
public partial record MyRecordVo;

public record BaseRecord;

[ValueObject]
public partial record MyDerivedRecordVo : BaseRecord;

[ValueObject<decimal>]
public partial struct MyDecimal;
