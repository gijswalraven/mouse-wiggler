using MouseWiggler;

ApplicationConfiguration.Initialize();

var startDisabled = args.Contains("--disabled");
Application.Run(new TrayApplicationContext(startDisabled));
