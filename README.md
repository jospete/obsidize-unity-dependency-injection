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

The below examples are pseudo-code for core concepts.

See the project samples for working code examples.

### Token Consumer Usage

For behaviours that want to use DI to get references, the easiest way is with an ```BehaviourInjectionContext``` instance:

```csharp
using Obsidize.DependencyInjection;
using UnityEngine;

// Note that only one Inject / InjectionListener attribute should be used per type.
// Otherwise, you will register multiple listeners on this class for the same value type.
public class Consumer : MonoBehaviour
{
	
	private BehaviourInjectionContext _injectionContext;
	private TokenAType _tokenA;
	private TokenBType _tokenB;
	private TokenCType _tokenC;
	private TokenDType _tokenD;
	
	private void Awake()
	{
		_injectionContext = new BehaviourInjectionContext(this)
			.Inject<TokenAType>(v => _tokenA = v)
			.Inject<TokenBType>(v => _tokenB = v, 10f) // can also pass a custom max-wait-time before the DI system will complain
			.InjectOptional<TokenBType>(v => _tokenB = v) // will not complain if no token is provided
			.Inject<TokenDType>(OnUpdateTokenD);
	}
	
	private void OnDestroy()
	{
		// Be sure to dispose the context when you're done with it to avoid memory leaks
		_injectionContext.Dispose();
	}
	
	private void OnUpdateTokenD(TokenDType value)
	{
		_tokenD = value;
		// do other stuff now that _tokenD is updated...
	}
}
```

### Token Source Usage

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
	
	// Optional override - defaults to casting the sub-class as the token
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