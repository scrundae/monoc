<body>
    <center>
        <h1>monoc</h1>
        <p>The <i>moddable </i>text editor for <b><i><u>you</u></i></b></p>
    </center>
    <hr>
    <h3>Modding using Lua</h3>
    <p>Did you know that MONOC has extensibility and automation support using the Lua programming language?</p>
    <p>Here's an example of a Lua script for MONOC</p>
    <pre><code>
        [<font color='blue'>Lua for MONOC Code Editor</font>]
        <font color='green'>-- makes a modal window</font>
        makemodal(<font color='brown'>"Hello, I am Modal McModalFace"</font>, <font color='brown'>"I am a modal"</font>)
        <font color='green'>-- a loop (that never runs) that will constantly write on the open document</font>
        <font color='blue'>while</font> 1+1==3 <font color='blue'>do</font>
            println(<font color='brown'>"This code will probably break MONOC to lag! Good thing it never runs!"</font>)
        <font color='blue'>end</font>
    </code></pre>
    <p>Isn't that cool? But here is the best part: Lua has extremely basic syntax and code, making it extremely fast and easy for literally ANYONE to code extensions for MONOC!</p>
    <p>Well now we know about extensions, but how do we actually make them usable?</p>
    <p>There are 2 ways:</p>
    <ul>
        <li>Save the file to mods\menubar; this makes them accessible from the "Mods" menu.</li>
        <li>Save the file to mods\startup; this makes them run on start.</li>
    </ul>
    <hr>
    <h3><u>You</u> can help contribute!</h3>
    <p>Visit our GitHub. You can help contribute there!</p>
    <p>The app is coded in .NET, with bits of HTML and Lua scattered in the mix.</p>
    <p>With your help, we can fix the stupid problems with this app, and make it an awesome text editor!</p>
    <hr>
        <p>You have reached the bottom of the page. Thanks for reading!</p>
