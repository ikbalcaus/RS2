import "dart:convert";
import "package:admin/utils/constants.dart";
import "package:flutter/material.dart";
import "package:http/http.dart" as http;

class AuthProvider extends ChangeNotifier {
  static String? email;
  static String? password;

  bool _isLoggedIn = false;
  bool get isLoggedIn => _isLoggedIn;

  set isLoggedIn(bool value) {
    _isLoggedIn = value;
    notifyListeners();
  }

  Future<http.Response> login(String email, String password) async {
    AuthProvider.email = email;
    AuthProvider.password = password;

    final response = await http.post(
      Uri.parse("${ApiHost.address}/users/login"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"email": email, "password": password}),
    );
    if (response.statusCode == 200) {
      final Map<String, dynamic> data = json.decode(response.body);
      final role = data["role"]["name"] as String;
      if (role != "Admin" && role != "Moderator") {
        return http.Response(
          "{\"error\": \"Forbidden\"}",
          403,
          headers: response.headers,
        );
      } else {
        isLoggedIn = true;
      }
    }
    return response;
  }

  Future<void> logout() async {
    isLoggedIn = false;
    notifyListeners();
  }
}
