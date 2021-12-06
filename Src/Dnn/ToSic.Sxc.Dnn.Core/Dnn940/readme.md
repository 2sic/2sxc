# Workaround for DNN 9.4 Breaking Changes

This is a special workaround for DNN 9.4 breaking changes. 

## How it works

Our code does the following
1. the setup-code runs automatically in `RegisterWebApiActivator.cs` - this happens, because our method `RegisterRoutes` is called automatically 
	1. it checks if we're running DNN 9.4+ 
	1. if this is the case, it injects our activator `WebApiHttpControllerActivator.cs` and give it the previous activator
1. our activater will
	1. try to activate using the DNN mechanisms - because it has the previous activator
	1. if that fails (returns null) it will use .net core dependency injection to activate dynamically compiled classes

Sidenote: since 2sxc is compiled for DNN 7.4.2+ it doesn't know some of the newer DNN 9 properties. So in this case it uses reflection to finish it's work.

## Why we did this

DNN 9.4 added .net core dependency injection - which is great - but they forgot an important use case: dynamically compiled WebApi endpoints. 

2sxc uses these in all custom Apps which have their own WebApis. The system is simple, as 2sxc compiles these on the fly and publishes them on HTTP. 

This works great, and to fix it it would have only needed a simple extra line in DNN. 
For currently unknown reasons, the DNN core team felt that this breaks dependency injection (to which we strongly disagree). 
Because of this, we decided to extend DNN to again support dynamically compiled endpoints. 

## See also

1. 2sxc github issue on this: https://github.com/2sic/2sxc/issues/1830
1. Discussion on DNN Platform: https://github.com/dnnsoftware/Dnn.Platform/issues/3045