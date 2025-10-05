import "package:flutter/material.dart";

class Globals {
  static const apiAddress = String.fromEnvironment(
    "_ngrokURL",
    defaultValue: "https://monarch-innocent-impala.ngrok-free.app",
  );
  static int pageIndex = 0;
  static const successMessage = "Action completed successfully";
  static const errorMessage = "Error: An error occurred";
  static const backgroundColor = Colors.blue;
}
