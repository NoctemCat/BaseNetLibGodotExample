using System.Runtime.CompilerServices;


using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Linq;
using System.Reflection;
using Godot.NativeInterop;


#if GODOT_WEB
using System.Runtime.InteropServices.JavaScript;
#endif
using System.Threading.Tasks;


namespace Sample;

public partial class Player : CharacterBody3D
{
	enum PlayerEnum
	{
		None,
		One,
	}
	enum PlayerEnumInt : int
	{
		None,
		One,
	}
	enum PlayerEnumLong : long
	{
		None,
		One,
	}

	[Export] Node3D Pivot { get; set; } = null!;
	[Export] float Sensitivity { get; set; } = 0.5f;
	[Export] float Speed { get; set; } = 5.0f;
	[Export] float JumpVelocity { get; set; } = 4.5f;
	[Export] PackedScene BallScene { get; set; } = null!;

	[Export] PlayerEnum one;
	[Export] PlayerEnumInt two;
	[Export] PlayerEnumLong three;

	[Export] public CustomRigidbody[]? BoidesArray { get; set; }
	[Export] public Godot.Collections.Array<CustomRigidbody>? BoidesGodotArray { get; set; }
	[Export] public Node3D[]? NodeArray { get; set; }

	public Player()
	{
		GD.Print("Player constructor");
		GD.Print($"Assembly has DisableRuntimeMarshalling: {Assembly.GetExecutingAssembly().GetCustomAttribute<DisableRuntimeMarshallingAttribute>() is not null}");

		Variant variant = new Node3D();
		if (variant.AsGodotObject() is not null)
		{
			GD.Print("Object not null");
		}
		if (variant.As<GodotObject>() is Node3D node)
		{
			node.QueueFree();
		}
		GD.Print("Player constructor after node");
		using Variant variantFloat = 20.0f;
		if (variantFloat.As<float>() is float floatValue)
		{
			GD.Print($"Read {floatValue}");
		}

		GD.Seed(123);
		GD.Print($"Randi 123: {GD.Randi()}");
		GD.Print($"Randf 123: {GD.Randf()}");
		GD.Print($"Randfn 123: {GD.Randfn(1.0, 10.0)}");
		GD.Print($"RandRange 123: {GD.RandRange(1.0, 10.0)}");

		string original = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Phasellus eu eros rhoncus, vehicula velit eget, laoreet dui. Cras sollicitudin justo vitae nibh condimentum, in mattis nunc tincidunt. Sed placerat viverra ex vitae laoreet. Donec id nisi id libero tincidunt fringilla vel at eros. Praesent eget pharetra odio. Vivamus tincidunt sagittis augue, ac sodales ante accumsan ut. Donec eu rhoncus nisi, sit amet tempor purus. Proin pharetra sed purus id finibus. Nullam tortor nisi, euismod at dapibus eu, commodo ac nunc.
Donec purus lorem, rhoncus in mattis vel, ultrices posuere lorem. Praesent scelerisque auctor varius. Suspendisse elit magna, eleifend non eleifend ut, finibus et sem. Cras interdum faucibus tellus nec feugiat. Fusce est augue, consectetur in nunc congue, vehicula cursus nisi. Etiam vel ligula ex. Nam vel odio quis.";
		byte[] bytes = Encoding.Unicode.GetBytes(original);
		byte[] compressed = GD.Compress(bytes);
		byte[] bytesBack = GD.Decompress(compressed, bytes.Length + 8);

		GD.Print($"Original: {bytes.Length}, Compressed: {compressed.Length}, Back: {bytesBack.Length}, Comparison: {bytes.SequenceEqual(bytesBack)}");

		GD.Print("Player constructor exit");
	}

	public override void _Ready()
	{
		GD.Print("Player _Ready");

		// RuntimeError: function signature mismatch check
		GD.Print($"godotsharp_variant_as_bool {((Variant)1).AsBool()}");
		GD.Print($"godotsharp_variant_as_int {((Variant)"1").AsInt64()}");
		GD.Print($"godotsharp_variant_as_float {((Variant)1).AsDouble()}");
		GD.Print($"godotsharp_variant_as_string int {((Variant)1234).AsString()}");
		GD.Print($"godotsharp_variant_as_string bool {((Variant)true).AsString()}");
		GD.Print($"godotsharp_variant_as_vector2 {((Variant)new Vector2I(1, 1)).AsVector2()}");
		GD.Print($"godotsharp_variant_as_vector2i {((Variant)new Vector2(1, 1)).AsVector2I()}");
		GD.Print($"godotsharp_variant_as_rect2 {((Variant)new Rect2I(1, 1, 0, 1)).AsRect2()}");
		GD.Print($"godotsharp_variant_as_rect2i {((Variant)new Rect2(1.5f, 1.5f, 1.5f, 0)).AsRect2I()}");
		GD.Print($"godotsharp_variant_as_vector3 {((Variant)new Vector2(1, 1)).AsVector3()}");
		GD.Print($"godotsharp_variant_as_vector3i {((Variant)new Vector2(1, 1)).AsVector3I()}");
		GD.Print($"godotsharp_variant_as_transform2d {((Variant)Transform3D.Identity).AsTransform2D()}");
		GD.Print($"godotsharp_variant_as_vector4 {((Variant)new Vector2(1, 1)).AsVector4()}");
		GD.Print($"godotsharp_variant_as_vector4i {((Variant)new Vector2(1, 1)).AsVector4I()}");
		GD.Print($"godotsharp_variant_as_plane {new Variant().AsPlane()}");
		GD.Print($"godotsharp_variant_as_quaternion {((Variant)Transform3D.Identity).AsQuaternion()}");
		GD.Print($"godotsharp_variant_as_aabb {new Variant().AsAabb()}");
		GD.Print($"godotsharp_variant_as_basis {((Variant)Transform3D.Identity).AsBasis()}");
		GD.Print($"godotsharp_variant_as_transform3d {((Variant)Transform2D.Identity).AsTransform3D()}");
		GD.Print($"godotsharp_variant_as_projection {((Variant)Transform2D.Identity).AsProjection()}");
		GD.Print($"godotsharp_variant_as_color {((Variant)1).AsColor()}");
		GD.Print($"godotsharp_variant_as_string_name {((Variant)"something").AsStringName()}");
		GD.Print($"godotsharp_variant_as_node_path {((Variant)"path1").AsNodePath()}");
		GD.Print($"godotsharp_variant_as_rid {((Variant)this).AsRid()}");
		GD.Print($"godotsharp_variant_as_callable {new Variant().AsCallable()}");
		GD.Print($"godotsharp_variant_as_signal {new Variant().AsSignal()}");
		GD.Print($"godotsharp_variant_as_dictionary {new Variant().AsGodotDictionary()}");
		GD.Print($"godotsharp_variant_as_array {((Variant)new byte[] { 1, 1, 1, 1 }).AsGodotArray()}");
		GD.Print($"godotsharp_variant_as_packed_byte_array ", (Variant)((Variant)new Godot.Collections.Array { 1, 1, 1, 1 }).AsByteArray());
		GD.Print($"godotsharp_variant_as_packed_int32_array ", (Variant)((Variant)new Godot.Collections.Array { 1, 1, 1, 1 }).AsInt32Array());
		GD.Print($"godotsharp_variant_as_packed_float32_array ", (Variant)((Variant)new Godot.Collections.Array { 1, 1, 1, 1 }).AsFloat32Array());
		GD.Print($"godotsharp_variant_as_packed_float64_array ", (Variant)((Variant)new Godot.Collections.Array { 1, 1, 1, 1 }).AsFloat64Array());
		GD.Print($"godotsharp_variant_as_packed_string_array ", (Variant)((Variant)new Godot.Collections.Array { 1, 1, 1, 1 }).AsStringArray());
		GD.Print($"godotsharp_variant_as_packed_vector2_array ", (Variant)((Variant)new Godot.Collections.Array { new Vector2(1, 1) }).AsVector2Array());
		GD.Print($"godotsharp_variant_as_packed_vector3_array ", (Variant)((Variant)new Godot.Collections.Array { new Vector2(1, 1) }).AsVector3Array());
		GD.Print($"godotsharp_variant_as_packed_vector4_array ", (Variant)((Variant)new Godot.Collections.Array { new Vector2(1, 1) }).AsVector4Array());
		GD.Print($"godotsharp_variant_as_packed_color_array ", (Variant)((Variant)new Godot.Collections.Array { 1, 1, 1, 1 }).AsColorArray());
		GD.Print($"godotsharp_color_from_ok_hsl {Color.FromOkHsl(1.0f, 0.0f, 1.0f)}");

		GD.Print($"Call(\"GetEnum1\") {Call("GetEnum1")}");
		GD.Print($"Call(\"GetEnum2\") {Call("GetEnum2")} ");
		GD.Print($"Call(\"GetEnum3\") {Call("GetEnum3")}");
		GD.Print($"Callable.Call(\"GetEnum1\") {new Callable(this, "GetEnum1").Call()}");

		GD.PrintS("BoidesArray", (Variant)(BoidesArray ?? new Variant()));
		GD.PrintS("BoidesGodotArray", (Variant)(BoidesGodotArray ?? new Variant()));
		GD.PrintS("NodeArray", (Variant)(NodeArray ?? new Variant()));

		Godot.Collections.Array arr = [1, 2];
		arr.Resize(4);
		GD.PrintS("arr", arr);

		Input.MouseMode = Input.MouseModeEnum.Captured;

		GD.Seed(123);
		GD.Print($"Is main thread {GodotThread.IsMainThread()}");
		GD.PrintS("Main Thread", Thread.CurrentThread.ManagedThreadId);
		TestThread();

		RunCrypro();
		if (OS.HasFeature("nothreads"))
		{
			// Causes too much deadlocks at rundom with multithreading
			RunHTTP();
		}
#if GODOT_WEB
		_ = TestAdvancedAsync();
#endif
	}


	PlayerEnum GetEnum1() => one;
	PlayerEnumInt GetEnum2() => two;
	PlayerEnumLong GetEnum3() => three;

	public override void _PhysicsProcess(double delta)
	{
		if (GetTree().Paused) { return; }

		ProcessMouse();

		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			var speed = Speed;
			if (Input.IsActionPressed("run"))
			{
				speed *= 1.5f;
			}
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		if (Input.IsActionJustPressed("Click"))
		{
			RigidBody3D ball = BallScene.Instantiate<RigidBody3D>();
			var forward = -Pivot.GlobalTransform.Basis.Z;
			ball.Position = GetParent<Node3D>().ToLocal(Pivot.GlobalTransform.Origin + forward * 2);
			GetParent().AddChild(ball);
		}
	}

	List<Vector2> mouseBuffer = [];

	private void ProcessMouse()
	{
		Vector2 mouseMotion = Vector2.Zero;
		foreach (var motion in mouseBuffer)
		{
			mouseMotion += motion;
		}
		mouseBuffer.Clear();

		RotateY(Mathf.DegToRad(-mouseMotion.X * Sensitivity));
		Pivot.RotateX(Mathf.DegToRad(-mouseMotion.Y * Sensitivity));
		Pivot.Rotation = Pivot.Rotation with { X = Mathf.Clamp(Pivot.Rotation.X, -Mathf.Pi / 2.0f, Mathf.Pi / 2.0f) };
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("pause"))
		{
			if (GetTree().Paused)
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
				GetTree().Paused = false;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
				GetTree().Paused = true;
			}
		}

		if (GetTree().Paused) { return; }

		if (@event is InputEventMouseMotion mouseMotion && !mouseMotion.ScreenRelative.IsZeroApprox())
		{
			mouseBuffer.Add(mouseMotion.ScreenRelative);
		}
	}

	private void RunCrypro()
	{
		var bytes = new byte[16];
		System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);
		GD.Print("Crypto result: " + BitConverter.ToString(bytes));
	}

	private async void RunHTTP()
	{
		CancellationTokenSource source = new();
		CancellationToken token = source.Token;
		int timeout = 2000;
		try
		{
			source.CancelAfter(timeout);
			using var client = new System.Net.Http.HttpClient();
			var response = await client.GetStringAsync("https://httpbin.org/get", token);
			GD.Print("HTTP result: " + response[..100]);
		}
		catch (OperationCanceledException)
		{
			GD.Print($"Tasks cancelled: timed out after {timeout}ms.");
		}
		catch (Exception e)
		{
			GD.Print("HTTP error: " + e.GetType().Name + ": " + e.Message);
		}
		finally
		{
			source.Dispose();
		}

	}

#if GODOT_WEB
	private async Task TestAdvancedAsync()
	{
		GD.Print("Hello, World!");

		var rand = new Random();
		GD.Print("Today's lucky number is " + rand.Next(100) + " and " + Guid.NewGuid());

		var start = DateTime.UtcNow;
		var timezonesCount = TimeZoneInfo.GetSystemTimeZones().Count;
		await JsDelay(100);
		var end = DateTime.UtcNow;
		GD.Print($"Found {timezonesCount} timezones in the TZ database in {end - start}");

		TimeZoneInfo utc = TimeZoneInfo.FindSystemTimeZoneById("UTC");
		GD.Print($"{utc.DisplayName} BaseUtcOffset is {utc.BaseUtcOffset}");

		try
		{
			TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
			GD.Print($"{tst.DisplayName} BaseUtcOffset is {tst.BaseUtcOffset}");
		}
		catch (TimeZoneNotFoundException tznfe)
		{
			GD.Print($"Could not find Asia/Tokyo: {tznfe.Message}");
		}
	}



	[JSImport("Sample.Test.add", "main.js")]
	internal static partial int Add(int a, int b);

	[JSImport("Sample.Test.delay", "main.js")]
	[return: JSMarshalAs<JSType.Promise<JSType.Void>>]
	internal static partial Task JsDelay([JSMarshalAs<JSType.Number>] int ms);

	[JSExport]
	internal static async Task PrintMeaning(Task<int> meaningPromise)
	{
		Console.WriteLine("Meaning of life is " + await meaningPromise);
	}

	[JSExport]
	internal static int TestMeaning()
	{
		var half = 21;
		// call back to JS via [JSImport]
		return Add(half, half);
	}

	[JSExport]
	internal static void SillyLoop()
	{
		Console.WriteLine("UtcNow is " + DateTime.UtcNow.Millisecond);
		// this silly method will generate few sample points for the profiler
		bool breakCond = false;
		for (int i = 1; i <= 500; i++)
		{
			try
			{
				for (int s = 0; s <= 500; s++)
				{
					try
					{
						if (DateTime.UtcNow.Millisecond == i + s)
						{
							Console.WriteLine("Time is " + s);
							breakCond = true;
							break;
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
				if (breakCond)
				{
					break;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}

	[JSExport]
	internal static bool IsPrime(int number)
	{
		if (number <= 1) return false;
		if (number == 2) return true;
		if (number % 2 == 0) return false;

		var boundary = (int)Math.Floor(Math.Sqrt(number));

		for (int i = 3; i <= boundary; i += 2)
			if (number % i == 0)
				return false;

		return true;
	}
#endif

	internal void TestThread()
	{
		Task.Run(TestThreadImpl);
	}
	internal async Task TestThreadImpl()
	{
		GD.Print(">>>> Entering Thread");
		if (OS.HasFeature("nothreads"))
		{
			await Task.Delay(2000);
		}
		else
		{
			Thread.Sleep(2000);
		}
		GD.PrintS(">>>> Hello Thread", Thread.CurrentThread.ManagedThreadId);
		if (OS.HasFeature("nothreads"))
		{
			await Task.Delay(2000);
		}
		else
		{
			Thread.Sleep(2000);
		}
		GD.Print(">>>> Exiting Thread");
	}
}
