# CircuitPerister sample

This is a demonstration of one possible way (of many) in which you can track per-browser-tab state so that it can survive across circuit restarts. This means that if the user reloads the page, or if the circuit becomes idle and is dropped by the server, the state will reappear when the page is reloaded.

**THIS IS ONLY AN EXAMPLE AND SHOULD NOT BE ASSUMED TO BE SUITABLE FOR ALL USE CASES**. Don't use this without understanding in detail how it works and verifying you are happy with how this uses server resources and how the state could be leaked across users if someone manages to extract a `sessionState` value from the user. No detailed security review has been undertaken. The code does not attempt to place a limit on how much state can be stored, other than what `MemoryCache` allows.

In this example the state is stored on the server in memory without serialization. Other possible approaches could include serialized storage inside the browser (e.g., in `sessionState`), or in a database such as Redis.
