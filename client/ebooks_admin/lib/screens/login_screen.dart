import "dart:convert";
import "package:ebooks_admin/utils/globals.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:ebooks_admin/providers/auth_provider.dart";
import "package:ebooks_admin/screens/master_screen.dart";
import "books_screen.dart";

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  Map<String, List<String>> _fieldErrors = {};
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  Future _login() async {
    setState(() {
      _fieldErrors.clear();
    });
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    final email = _emailController.text.trim();
    final password = _passwordController.text;
    final response = await authProvider.login(email, password);
    if (response.statusCode == 200) {
      if (mounted) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => const BooksScreen()),
        );
      }
    } else if (response.statusCode == 403) {
      setState(
        () => _fieldErrors = {
          "general": ["Only admin and moderator can access the dashboard"],
        },
      );
    } else {
      try {
        final Map<String, dynamic> data = json.decode(response.body);
        final errors = data["errors"] as Map<String, dynamic>;
        setState(() {
          _fieldErrors = errors.map((key, value) {
            final List<String> messages = List<String>.from(value);
            return MapEntry(key.toLowerCase(), messages);
          });
        });
      } catch (ex) {
        setState(() {
          _fieldErrors = {
            "general": ["Unknown error occurred"],
          };
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      child: Center(
        child: Transform.translate(
          offset: Offset(0, -60),
          child: ConstrainedBox(
            constraints: const BoxConstraints(maxWidth: 400),
            child: Form(
              key: _formKey,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  Image.asset(
                    "assets/images/login.png",
                    width: 250,
                    height: 250,
                  ),
                  const SizedBox(height: 4),
                  TextFormField(
                    controller: _emailController,
                    decoration: InputDecoration(
                      prefixIcon: const Icon(Icons.email_outlined),
                      labelText: "Email",
                      errorText: _fieldErrors["email"]?.first,
                    ),
                    keyboardType: TextInputType.emailAddress,
                  ),
                  const SizedBox(height: 10),
                  TextFormField(
                    controller: _passwordController,
                    decoration: InputDecoration(
                      prefixIcon: const Icon(Icons.lock_outline),
                      labelText: "Password",
                      errorText: _fieldErrors["password"]?.first,
                    ),
                    obscureText: true,
                  ),
                  if (_fieldErrors.containsKey("general")) ...[
                    const SizedBox(height: 12),
                    ..._fieldErrors["general"]!.map(
                      (msg) => Text(
                        msg,
                        style: const TextStyle(color: Colors.red, fontSize: 13),
                      ),
                    ),
                  ],
                  const SizedBox(height: 20),
                  SizedBox(
                    width: double.infinity,
                    child: ElevatedButton(
                      onPressed: () async => await _login(),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Globals.backgroundColor,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(
                            Globals.BorderRadius,
                          ),
                        ),
                      ),
                      child: const Text(
                        "Login",
                        style: TextStyle(
                          fontSize: 16,
                          color: Colors.white,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
