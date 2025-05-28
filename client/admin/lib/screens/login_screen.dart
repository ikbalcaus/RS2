import "dart:convert";
import "package:flutter/material.dart";
import "package:http/http.dart" as http;

Future<void> _login(String email, String password) async {
  final String basicAuth = "Basic " + base64Encode(utf8.encode('$email:$password'));

  final response = await http.post(
    Uri.parse("https://localhost:7210/users/login"),
    headers: {
      "Authorization": basicAuth,
      "Content-Type": "application/json",
    },
    body: jsonEncode({
      "email": email,
      "password": password,
    }),
  );

  print("Status code: ${response.statusCode}");
  print("Body: ${response.body}");
}

class LoginScreen extends StatelessWidget {
  LoginScreen({super.key});
  TextEditingController _emailController = new TextEditingController();
  TextEditingController _passwordController = new TextEditingController();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Center(
          child: Container(
            constraints: const BoxConstraints(maxHeight: 400, maxWidth: 400),
            child: Column(
              children: [
                Image.asset("assets/images/books.png", height: 100, width: 100,),
                TextField(controller: _emailController, decoration: InputDecoration(labelText: "Email", prefixIcon: Icon(Icons.email))),
                TextField(controller: _passwordController, decoration: InputDecoration(labelText: "Password", prefixIcon: Icon(Icons.password))),
                ElevatedButton(
                  onPressed: () async {
                    await _login(_emailController.text, _passwordController.text);
                  },
                  child: Text("Login")
                )
              ],
            ),
          ),),
      ),
    );
  }
}
