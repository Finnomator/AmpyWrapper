using System.Diagnostics;

namespace AmpyWrapper;

public class Ampy {

    public int ComPort { get; }

    public Ampy(int comPort) {
        ComPort = comPort;
    }

    private Process CreateAmpyProcess() {
        return new() {
            StartInfo = new() {
                FileName = "ampy",
                Arguments = $"-p COM{ComPort} ",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
        };
    }

    private async Task RunAmpyNoOutput(string arguments) {
        Process process = CreateAmpyProcess();
        process.StartInfo.Arguments += arguments;
        process.Start();
        await process.WaitForExitAsync();
    }

    private async Task<AmpyOutput> RunAmpy(string arguments) {
        Process process = CreateAmpyProcess();
        process.StartInfo.Arguments += arguments;
        process.Start();
        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        return new(output, error);
    }

    /// <summary>
    /// Retrieve a file from the board.
    /// </summary>
    /// <example>
    /// <code>
    /// string fileContent = await GetFileContent("main.py");
    /// </code>
    /// </example>
    /// <param name="remoteFile">Path to the file on the board.</param>
    /// <returns>AmpyOutput that contains the file content.</returns>
    public async Task<AmpyOutput> GetFileContent(string remoteFile) => await RunAmpy($"get {remoteFile}");

    /// <summary>
    /// Download a file from the board.
    /// </summary>
    /// <example>
    /// <code>
    /// await GetFileContent("main.py", "C:\main.py");
    /// </code>
    /// </example>
    /// <param name="remoteFile">Path to the file on the board.</param>
    /// <param name="localFile">Local path to save the file to.</param>
    /// <returns>AmpyOutput that contains the file content.</returns>
    public async Task DownloadFile(string remoteFile, string localFile) => await RunAmpyNoOutput($"get {remoteFile} {localFile}");

    /// <summary>
    /// Create a directory on the board.
    /// </summary>
    /// <example>
    /// <code>
    /// await CreateDirectory("/not/existing/directory/", makeParents = true);
    /// </code>
    /// </example>
    /// <param name="directory">Directory path.</param>
    /// <param name="existsOkay">Ignore if the directory already exists.</param>
    /// <param name="makeParents">Create any missing parents.</param>
    public async Task CreateDirectory(string directory, bool existsOkay = false, bool makeParents = false) {
        string arguments = $"mkdir {directory}";
        if (existsOkay)
            arguments += " --exists-okay";
        if (makeParents)
            arguments += " --make-parents";
        await RunAmpyNoOutput(arguments);
    }

    /// <summary>
    /// List contents of a directory on the board.
    /// </summary>
    /// <example>
    /// <code>
    /// string rootPaths = await ListDirectory();
    /// </code>
    /// </example>
    /// <param name="directory">Path to the directory.</param>
    /// <param name="longFormat">Print long format info including size of files. Note the size of directories is not supported and will show 0 values.</param>
    /// <param name="recursive">Recursively list all files and (empty) directories.</param>
    /// <returns>A string of files and folders.</returns>
    public async Task<AmpyOutput> ListDirectory(string directory = "/", bool longFormat = false, bool recursive = false) {
        string arguments = $"ls {directory}";
        if (longFormat)
            arguments += " -l";
        if (recursive)
            arguments += " -r";
        return await RunAmpy(arguments);
    }

    /// <summary>
    /// Put a file or folder and its contents on the board.
    /// </summary>
    /// <example>
    /// To upload a main.py from the current directory to the board's root:
    /// <code>await Upload("C:/main.py")</code>
    /// To upload a board_boot.py from a ./foo subdirectory and save it as boot.py in the board's root:
    /// <code>await Upload("./foo/board_boot.py", "boot.py")</code>
    /// To upload a local folder adafruit_library and all of its child files/folders as an item under the board's root:
    /// <code>await Upload("adafruit_library")</code>
    /// To put a local folder adafruit_library on the board under the path /lib/adafruit_library on the board:
    /// <code>await Upload("adafruit_library", "/lib/adafruit_library")</code>
    /// </example>
    /// <param name="local">Local file or folder path.</param>
    /// <param name="remote">Remote file or folder path.</param>
    public async Task Upload(string local, string? remote = null) {
        string arguments = $"put {local}";
        if (remote != null)
            arguments += $" {remote}";
        await RunAmpyNoOutput(arguments);
    }

    /// <summary>
    /// Remove a file from the board.
    /// </summary>
    /// <example>
    /// <code>await RemoveFile("main.py")</code>
    /// </example>
    /// <param name="remoteFile">Remote file path.</param>
    public async Task RemoveFile(string remoteFile) => await RunAmpyNoOutput($"rm {remoteFile}");

    /// <summary>
    /// Forcefully remove a folder and all its children from the board.
    /// </summary>
    /// <example>
    /// <code>await RemoveDirectory("adafruit_library")</code>
    /// </example>
    /// <param name="remoteFolder">Remote folder path.</param>
    /// <param name="missingOkay">Ignore if the directory does not exist.</param>
    public async Task RemoveDirectory(string remoteFolder, bool missingOkay = false) {
        string arguments = $"rmdir {remoteFolder}";
        if (missingOkay)
            arguments += " --missing-okay";
        await RunAmpyNoOutput(arguments);
    }

    /// <summary>
    /// Perform soft reset/reboot of the board.
    /// </summary>
    /// <param name="resetMode">The reset mode.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public async Task Reset(ResetMode resetMode = ResetMode.Repl) {
        string arguments = "reset";

        arguments += resetMode switch {
            ResetMode.Hard => " --hard",
            ResetMode.Repl => " --repl",
            ResetMode.Safe => " --safe",
            ResetMode.Bootloader => " --bootloader",
            _ => throw new ArgumentOutOfRangeException(nameof(resetMode), resetMode, null)
        };

        await RunAmpyNoOutput(arguments);
    }

    /// <summary>
    /// Upload and run a script, wait for it to finish and return its output.
    /// </summary>
    /// <param name="localFile">Path to local file that will be executed</param>
    /// <returns>AmpyOutput containing the output of the executed file.</returns>
    public async Task<AmpyOutput> Run(string localFile) => await RunAmpy($"run {localFile}");

    /// <summary>
    /// Upload and run a script and do not wait for it to finish.
    /// </summary>
    /// <param name="localFile">Path to local file that will be executed</param>
    public async Task RunNoWait(string localFile) => await RunAmpyNoOutput($"run -n {localFile}");

    /// <summary>
    /// Upload and run the code and receive output.
    /// </summary>
    /// <param name="localFile">Path to local file that will be executed</param>
    /// <returns>A process which can be used to retrieve the output via its events</returns>
    public Process RunWithStreamOutput(string localFile) {
        Process process = CreateAmpyProcess();
        process.StartInfo.Arguments += $"run -s {localFile}";
        process.StartInfo.RedirectStandardInput = true;
        process.EnableRaisingEvents = true;
        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();
        return process;
    }
}
