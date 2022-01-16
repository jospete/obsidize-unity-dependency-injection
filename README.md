# Obsidize Dependency Injection

Utilities for enhanced unity app control flow via DI tokens.

**Note**: this is an over-engineered solution meant for projects with many sub-systems that
need to interact with each other. If you're working on a small project with 10 or less systems,
you probably don't need this.

The main benefits of this module are:

1. Faster than traditional "search the scene" methods (by alot) when looking for a shared object
2. Duplicate instance disambiguation / handling out-the-box
3. Better alternative to the singleton pattern (in my opinion)
4. Elimination of circular assembly definition reference errors (when used with bridge interface tokens)
5. Elimination of dependency creation racing conditions (i.e. module A needs module B, but module A was created before module B)
6. Allows for runtime token reference hot-swapping (i.e. overwriting a token value will update all watchers)
7. Independent sub-system operation - systems don't know when (or if) a token will be provided, and must operate in isolation with this in mind (their default behaviour should be to do "nothing" instead of explode on reference error)
8. Allows for true prefab isolation by eliminating shared scene references from the inspector

TL;DR:

```csharp

// Bad - searches entire scene heirarchy
CustomScript scriptRef = FindObjectOfType<CustomScript>();

// Good - direct reference to explicitly provided instance 
CustomScript scriptRef = Injector.Main.Get<CustomScript>();

// Better - also allows for interface-based povisioning to further decouple modules
ICustomBehaviour interfaceRef = Injector.Main.Get<ICustomBehaviour>();
```

## Installation

This git repo is directly installable via the unity package manager.
Simply paste the repo url into the package manager "add" menu and unity will do the rest.

## Usage

See the project samples for working code examples.

## Token Sources

For custom non-behaviour tokens, extend ```InjectionTokenSource```:

```csharp
using Obsidize.DependencyInjection;
using UnityEngine;

public interface MyCustomTokenType
{
	int SomeData { get; }
	void DoTheThing();
}

[DisallowMultipleComponent]
public class CustomTokenProvider : InjectionTokenSource<MyCustomTokenType>, MyCustomTokenType
{

	[SerializeField] private int _someData;

	public int SomeData => _someData;
	
	protected override MyCustomTokenType GetInjectionTokenValue() => this;

	public void DoTheThing()
	{
		Debug.Log("Did the thing");
	}
}
```

If you want to provide an existing behaviour as a token, extend ```SiblingComponentInjectionTokenSource```
and add this behaviour to the GameObject containing the original behaviour:

```csharp
using Obsidize.DependencyInjection;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerHealth 
{

	[SerializeField] private int _value;
	
	public int Value => _value;	
}

[DisallowMultipleComponent]
public class PlayerHealthProvider : SiblingComponentInjectionTokenSource<PlayerHealth>
{
}
```