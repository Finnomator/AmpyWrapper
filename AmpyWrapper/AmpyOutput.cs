namespace AmpyWrapper {
    
    public readonly struct AmpyOutput {
        public string Output { get; }
        public string Error { get; }

        public AmpyOutput(string output, string error) {
            Output = output;
            Error = error;
        }
    }
}
