class ApiHost {
  static const address = String.fromEnvironment(
    "API_HOST",
    defaultValue: "https://localhost:7210",
  );
}
