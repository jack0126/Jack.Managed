# Jack.Managed

#仿 spring-boot设计模式编程的工具类

using System;

using Jack.Managed;

namespace Demo

{

  [AutoManagedComponents]
  
  class Demo
  
  {
  
    static void Main(string[]args)
    {
      ManagedContext.Initializer<Demo>();
    }
  
  }
  [Component]
  public class TestComponent
  {
  
    public string Msg { get; set; } = “this is message!”；
    
  }
  [Component]
  public class TestComponent2 : IComponentInitializer
  
  {
    [Autowired]
    public TestComponent Tc { get; set; }
    
    public void ComponentInitializer()
    
    {
    
      Console.WriteLine(Tc.Msg);
      
    }
  
  }
  
}
