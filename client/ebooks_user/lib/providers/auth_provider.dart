import "dart:convert";
import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";
import "package:http/http.dart" as http;

class AuthProvider extends ChangeNotifier {
  static String? email;
  static String? password;
  static bool isLoggedIn = false;
  static int userId = 0;

  Future login(String email, String password) async {
    AuthProvider.email = email;
    AuthProvider.password = password;
    final response = await http.post(
      Uri.parse("${Globals.apiAddress}/users/login"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({"email": email, "password": password}),
    );
    if (response.statusCode == 200) {
      isLoggedIn = true;
    }
    return response;
  }

  Future logout() async {
    AuthProvider.email = null;
    AuthProvider.password = null;
    isLoggedIn = false;
    notifyListeners();
  }
}
