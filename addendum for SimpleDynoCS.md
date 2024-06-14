# additional document for SimpleDynoCS
<p align="right">
Last Update
06/14/2024 by vmx17
</p>

## Changes for C#
`SimpleDynoCS` was basically converted by code converter but I add some changes to modernization. Here I show them.

- The `Main` class is separated into `Program` and `SimpleDyno` class. The `SimpleDyno` is partial class of main form (so the main form class is `SimpleDyno` not `MainF` nor `Main`). The `Program` class holds `MainI` property as static `SimpleDyno` instance. So main class can be specified as `Program.MainI` in general. (It corresponds to `this` if it is not static.)
- Directory for setting files is moved to `C:\Users\your_dir\AppData\Roaming\SimpleDyno"` as default location. The path is gotten by using APIs so it may vary with your configuration. It no longer uses `Program Files` nor `C:\` directory for such purpose.
- some names of variables are changed. The name start with "m_" is private member, start with "s_" is static variables. The private member variables are made to public by property definitions (if required). These change has not been finished. I'll change them one by one.
- `#define SOMEVALUE = 1` directives were converted to like;  
	```C#
	#define A
	...
	#if A
		// This code will be compiled if A is defined.
	#else
		// This code will be compiled if A is not defined.
	#endif
	...
	```
	with this change, I cannot specify number in preprocessor symbol. So far such number did not appeared. If you want `#else` part active, just comment out `#define ...`.
- any event handlers those assigned to MainForm (the name is "SimpleDyno") events are moved to `InitializeComponent()` as VS default way.