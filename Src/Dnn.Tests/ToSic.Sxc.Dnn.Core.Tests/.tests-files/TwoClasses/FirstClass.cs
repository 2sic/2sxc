public class FirstClass
{
    // Property that returns other class
    public SecondClass SecondClass { get; set; }

    public FirstClass()
    {
        // Initialize SecondClass
        SecondClass = new SecondClass();
    }
}