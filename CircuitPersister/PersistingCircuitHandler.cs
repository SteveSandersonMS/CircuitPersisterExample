using Microsoft.AspNetCore.Components.Server.Circuits;

namespace CircuitPersister;

public class PersistingCircuitHandler : CircuitHandler
{
    private Guid _id;
    private CircuitStateStore _stateStore;

    // These are the values that we'll restore if a component wants to use them
    private Dictionary<string, object?>? _loadedValues;

    // These are the values that we'll save when the circuit becomes idle
    // This is a separate collection from _loadedValues because we only want to recover each value
    // once, after which it's no longer available. That is to avoid old state reappearing unexpectly
    // as the user navigates around.
    private Dictionary<string, PersistableValue>? _activeValues = new();

    public PersistingCircuitHandler(CircuitStateStore store)
    {
        _stateStore = store;
    }

    public Guid InitializeAsNew()
    {
        _id = Guid.NewGuid();
        _loadedValues = new();
        return _id;
    }

    public async Task InitializeAsExistingAsync(Guid id)
    {
        _id = id;
        _loadedValues = await _stateStore.LoadAsync(_id);
    }

    public PersistableValue<T> Take<T>(string key)
    {
        var typedValue = _loadedValues!.Remove(key, out var value) ? (T?)value : default;
        var result = new PersistableValue<T>(typedValue);
        _activeValues![key] = result;
        return result;
    }

    // In this case, we save the data whenever the circuit is closed. With a bit more code we could use
    // CreateInboundActivityHandler to detect when circuits become idle, so that we only save their data
    // when they are idle and not when a user intentionally closes the circuit (e.g., navigating away)
    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
        =>_stateStore.SaveAsync(_id, _activeValues!.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.GetValue()));
}

public class PersistableValue<T> : PersistableValue
{
    internal PersistableValue(T? value)
    {
        Value = value;
    }

    public T? Value { get; set; }

    public override object? GetValue() => Value;
}

public abstract class PersistableValue
{
    public abstract object? GetValue();
}
