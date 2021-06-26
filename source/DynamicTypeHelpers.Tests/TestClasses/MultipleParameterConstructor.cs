namespace DynamicTypeHelpers.Tests.TestClasses
{
    public class MultipleParameterConstructor
    {
        public MultipleParameterConstructor(int value1, int value2, string otherValue, IObjectUnderTest referenceVale)
        {
            Value1 = value1;
            Value2 = value2;
            OtherValue = otherValue;
            ReferenceVale = referenceVale;
        }

        public int Value1 { get; }
        public int Value2 { get; }
        public string OtherValue { get; }
        public IObjectUnderTest ReferenceVale { get; }
    }
}
