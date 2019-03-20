### Dependency Injection

Captura is heavily based on Dependency Injection (DI) and uses [Ninject](http://www.ninject.org/) as the IoC container.
You might want to read about DI if you're unfamiliar with it.

> THIS DOCUMENT IS INCOMPLETE AND MAY NOT PROVIDE GOOD UNDERSTANDING OF DI IN ITS CURRENT STATE. YOU ARE RECOMMENDED TO READ ABOUT DI FROM OTHER SOURCES.

Let's say we have a class `Car` depends on class `DieselEngine`.

```csharp
class Car
{
  DieselEngine engine = new DieselEngine();

  public Car()
  {
    engine.Start();
  }
}
```

Now, if we've to change the `engine` to a different model, we will have to heavily modify the `Car` class.
So, we introduce an interface `IEngine` which is implemented by `DieselEngine` and other engine classes.

```csharp
interface IEngine
{
  void Start();
}
```

Now, the `Car` class will store `engine` as an `IEngine`. This will make it easier to change just the engine and rest of the code will remain same.

```csharp
class Car
{
  IEngine engine = new DieselEngine();

  public Car()
  {
    engine.Start();
  }
}
```

Now, suppose if we want to use a different type of engine, we cannot do so currently without modifying the `Car` class because `DieselEngine` is hardcoded in it. Instead if we took `IEngine` as a constructor parameter, we could easily provide a different engine like `ElectricEngine`.

```csharp
class Car
{
  IEngine engine;

  public Car(IEngine Engine)
  {
    engine = Engine;

    engine.Start();
  }
}
```

```csharp
var carWithDieselEngine = new Car(new DieselEngine());
var carWithElectricEngine = new Car(new ElectricEngine());
```

This is a form of dependency injection called **Constructor Injection**. There are more forms of dependency injection like **Setter Injection** but they are not used in Captura, so will not be discussed here.

Now, imagine a real Car and all the parts that will be required to make it. If we start to provide everything with dependency injection, our code will be full of `new` statements. The provided dependencies may in turn depend on other objects like shown for the `Brakes` object.

```csharp
var car = new Car(
  new Brakes(new Caliper(), new LightSwitch(), new BrakeRotor())
  new DieselEngine(),
  new OffRoadWheels());
```

Now, suppose a library could do the work of creating these dependencies and passing them to the `Car` object. That's what an Inversion of Control (IoC) container like Ninject does.

```csharp
var car = ServiceProvider.Get<Car>();
```