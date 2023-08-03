namespace AmpyWrapper {
    public enum ResetMode {
        /// <summary>Reboot into the bootloader</summary>
        Bootloader,
        /// <summary>Perform a hard reboot, including running init.py</summary>
        Hard,
        /// <summary>Perform a soft reboot, entering the REPL</summary>
        Repl,
        /// <summary>Perform a safe-mode reboot. User code will not be run and the filesystem will be writeable over USB</summary>
        Safe
    }
}
