# Execution Context (Experimental)

Many services etc. need to know about the current context.

This to get the latest Block, Settings, Resources etc.

ATM we have the CodeApiService which originally was meant to provide all the APIs
needed by Razor, WebApi etc. 
...but has since been used by all services to get their context.

These interfaces here MUST be implemented by the CodeApiService,
but to use them will require an explicit cast. 