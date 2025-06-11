import "dart:convert";
import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";

class Helpers {
  static void showSuccessMessage(BuildContext context, [String? message]) {
    message ??= Globals.successMessage;
    if (context.mounted) {
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text(message)));
    }
  }

  static void showErrorMessage(BuildContext context, Object ex) {
    String errorMessage = Globals.errorMessage;
    if (context.mounted) {
      if (ex.toString().trim().isNotEmpty) {
        try {
          final errorJson = jsonDecode(ex.toString()) as Map<String, dynamic>;
          if (errorJson.containsKey("errors")) {
            final errors = errorJson["errors"] as Map<String, dynamic>;
            errorMessage = errors.entries
                .map(
                  (entry) =>
                      "${entry.key}: ${(entry.value as List).join(", ")}",
                )
                .join("\n");
          } else {
            errorMessage = ex.toString();
          }
        } catch (_) {
          errorMessage = ex.toString();
        }
      }
      if (context.mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text(errorMessage)));
      }
    }
  }
}
