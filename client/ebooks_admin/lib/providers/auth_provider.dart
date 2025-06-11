import "dart:convert";
import "package:ebooks_admin/utils/constants.dart";
import "package:flutter/material.dart";
import "package:http/http.dart" as http;

class AuthProvider extends ChangeNotifier {
  static String? email;
  static String? password;
  static String? role;
  bool isLoggedIn = false;

  Future login(String email, String password) async {
    AuthProvider.email = email;
    AuthProvider.password = password;
    final response = await http.post(
      Uri.parse("${Constants.apiAddress}/users/login"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"email": email, "password": password}),
    );
    if (response.statusCode == 200) {
      final Map<String, dynamic> data = json.decode(response.body);
      role = data["role"]["name"] as String?;
      if (role != "Admin" && role != "Moderator") {
        return http.Response(
          "{\"Error\": \"Forbidden\"}",
          403,
          headers: response.headers,
        );
      } else {
        isLoggedIn = true;
      }
    }
    return response;
  }

  Future logout() async {
    isLoggedIn = false;
    notifyListeners();
  }
}
