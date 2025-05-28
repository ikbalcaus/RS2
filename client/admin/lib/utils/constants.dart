class ApiHost {
  static const address = String.fromEnvironment("API_HOST", defaultValue: "https://localhost");
  static const port = String.fromEnvironment("API_PORT", defaultValue: "7210");
}
