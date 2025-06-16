import "package:flutter/material.dart";

class Globals {
  static const apiAddress = String.fromEnvironment(
    "API_HOST",
    defaultValue: "https://localhost:7210",
  );
  static const successMessage = "Action completed successfully";
  static const errorMessage = "Error: An error occurred";
  static const spacing = 10.0;
  static const backgroundColor = Colors.blueAccent;
  static const color = Colors.white;
}
