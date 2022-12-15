//class A
//{
//    protected int x;
//    public A()
//    {
//        x = 2;
//    }
//    public virtual void Print()
//    {
//        Console.WriteLine("4");

//    }

//}
class B 
{
    static int x = 1;
    public B()
    {
        x++;
    }
    static  B()
    {
        x += 3;
    }
    int a = 6;
    public  void Print()
    {
        Console.WriteLine($"{x},{++x}");
    }
}
record A
{
    static int x = 1;
    
    public A() => x = 5;
    static A() => x = 4;
    public void Print() => Console.WriteLine($"{x},{++x}");
}
delegate void CallBack(string s);
internal class Program
{
    public event CallBack evt;
    static int GetValue( out int a, out int b,  int c)
    {
        a = ++c;
        b = a++;
        return c;
    }
    private static void Main(string[] args)
    {
        B a = new B();
        a.Print();
    }
}
