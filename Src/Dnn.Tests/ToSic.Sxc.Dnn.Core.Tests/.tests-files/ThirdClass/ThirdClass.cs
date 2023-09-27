public class ThirdClass
{
    // Property that returns other class
    public SecondClass SecondClass { get; set; }

    public ThirdClass()
    {
        // Initialize SecondClass
        SecondClass = new SecondClass();
    }
}