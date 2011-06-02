Synoptic - Making Console Applications Neater, Easier and Faster
================================================================

This library can be used to make writing .NET console applications easier. Conceptually, you can think of it as routing for the command line. You can specify which classes and methods in your application can be accessed via the command line and synoptic does the rest.

The excellent [Mono.Options](http://mono-project.com/Main_Page) is used internally to do the command line parsing.

The bits and pieces
-------------------

In Synoptic, there are two main concepts to understand that will make things easier down the line:

*Commands*

These map to classes and are a top-level structure for defining methods that can be accessed from the command line. If we are using the git cli as an example, "add" or "commit" might be considered commands.

*Actions*

Each command defines one or more actions. These map to methods within your command class. The parameters defined for your method become inputs on the command line. This should all become clear in the following examples.

Running your first Synoptic application
---------------------------------------

Typically, when you create a new Console Application using Visual Studio, you end up with an entry point that is not terribly interesting but looks something like:
    
    namespace ConsoleApplication1
    {
        class Program
        {
            static void Main(string[] args)
            {
            }
        }
    }

Add a reference to Synoptic.dll and and change the entry point to this:

    using Synoptic;

    namespace ConsoleApplication1
    {
        class Program
        {
            static void Main(string[] args)
            {
                new CommandRunner().Run(args);
            }
        }
    }

That wasn't so bad now was it? You *should* be able to compile and run it. The output should look similar to the following:

*There are currently no commands defined.
  
Please ensure commands are correctly defined and registered within Synoptic using the [Command] attribute.*

Not that interesting... yet.

Adding your first command
-------------------------
Add the following code somewhere within your new console application project (it doesn't matter where as long as it's in the same assembly as your console executable):

	[Command]
    public class MyCommand
    {
        [CommandAction]
        public void SayHello(string message)
        {
            Console.WriteLine("Hello " +  message);
        }
    }

You may need to add the relevant "using" statements if you chose to put this class in a separate file.

Compile and run your application and you should see output similar to:

    Usage: ConsoleApplication1 COMMAND ACTION [ARGS]

    say-hello

Congratulations - you have now exposed your first command via the command line!

Executing your first command
----------------------------
Executing a synoptic command always follows the pattern of:  

    <path to exe> COMMAND ACTION <params[optional]>

Ignoring parameters for now, we can execute our command as follows:

    ConsoleApplication1 my-command say-hello

It should show the following output:

    Hello

Note: to see all the actions and parameters available for a particular command, type in the command name without the action name,
e.g.

    ConsoleApplication1 my-command say-hello

Passing parameters
------------------
Execute your command by running the following:  
      
    ConsoleApplication1 my-command say-hello --message=world

It should show the following output:

    Hello world

Parameter formats
-----------------
If you are familiar with Mono.Options it should come as no surprise that you can pass parameters in various formats. The remarks within the [NDesk.Options documentation](http://www.ndesk.org/doc/ndesk-options/NDesk.Options/OptionSet.html#T:NDesk.Options.OptionSet:Docs:Remarks) (on which Mono.Options is based) has a good definition of the various formats.

A note about commands
---------------------
In order to have a method interpreted as a valid command action that is understood by Synoptic, the following criteria must be met:

  - It must be contained within a class with the "[Command]" attribute.
  - The method must be public
  - The method must only have primitive parameters (it can have no parameters too) 
  - The method must be decorated with the [CommandAction] attribute.
  - If there are duplicate command names (either through method naming or aliasing), the command which will be resolved for execution is undefined.

You will notice that the command names specified in the usage help is formatted as lower-case with hyphens; the command will be resolved when you refer to it using the formatted or unformatted name and is case-insensitive.

Advanced usage
--------------
Although Synoptic was designed to cover the most common use-cases with minimal effort, there are times when you want more control.

####Specifying commands manually
In the above example, the commands were in the same assembly as the executable which "just works". If there are commands defined  in other assemblies, synoptic can be initialised as follows:

    new CommandRunner()
        .WithCommandsFromAssembly(MyOtherAssembly)
        .Run(args);

To register commands only defined within a specific class, the following can be used:

    new CommandRunner()
        .WithCommandsFromType<MyType>()
        .Run(args);

These calls can be chained together as necessary.

####Customising command usage
By convention, a formatted version of the method name is used as the command name. In the above example, the method named *SayHello* yielded the action name *say-hello* (either can be used to invoke the command).

The [Command] attribute can be used to customise the name and help text as follows:

    [Command(Name = "mycommand", 
        Description = "Does things from the command line.")]

If you view the usage of your application by running it with no arguments you will see that the command name has changed and there is a description of the command alongside the command name.

####Customising parameter usage
Parameters can be customised in much the same was as commands using the [CommandParameterAttribute].

    public void SayHello([CommandParameter(Prototype = "m|msg", Description = "The message to show.")] string message)

An additional feature only currently supported by parameters is the *Prototype* property replaces the *Name* property. This allows you to specify multiple shortcuts for the same parameter. In the above example, the message could specified using either *--m=mymessage* or *--msg=mymessage*. If a custom prototype is not supplied, the parameter name is used. In addition, the DefaultValue property can be used for a parameter to specify what the value should be if omitted from the command line.

Note: This uses the Mono.Options prototype functionality and, as such, similar rules of formatting apply.
