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
  bool _isLoading = false;
  Map<String, List<String>> _fieldErrors = {};
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  Future _login() async {
    setState(() {
      _isLoading = true;
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
    setState(() => _isLoading = false);
  }

  @override
  Widget build(BuildContext context) {
    return MasterScreen(
      child: Center(
        child: SizedBox(
          width: 400,
          child: Form(
            key: _formKey,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                TextFormField(
                  controller: _emailController,
                  decoration: InputDecoration(
                    labelText: "Email",
                    border: const OutlineInputBorder(),
                    isDense: true,
                    contentPadding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 14,
                    ),
                    errorText: _fieldErrors["email"]?.first,
                  ),
                  keyboardType: TextInputType.emailAddress,
                ),
                const SizedBox(height: 16.0),
                TextFormField(
                  controller: _passwordController,
                  decoration: InputDecoration(
                    labelText: "Password",
                    border: const OutlineInputBorder(),
                    isDense: true,
                    contentPadding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 14,
                    ),
                    errorText: _fieldErrors["password"]?.first,
                  ),
                  obscureText: true,
                ),
                const SizedBox(height: 2),
                _fieldErrors.containsKey("general")
                    ? Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: _fieldErrors["general"]!
                            .map(
                              (msg) => Padding(
                                padding: const EdgeInsets.only(top: 2.0),
                                child: Text(
                                  msg,
                                  style: const TextStyle(
                                    color: Colors.red,
                                    fontSize: 12,
                                    height: 1.4,
                                  ),
                                ),
                              ),
                            )
                            .toList(),
                      )
                    : const SizedBox.shrink(),
                const SizedBox(height: 16.0),
                _isLoading
                    ? const CircularProgressIndicator()
                    : SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: () async {
                            await _login();
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Globals.backgroundColor,
                            shape: const RoundedRectangleBorder(
                              borderRadius: BorderRadius.zero,
                            ),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                          ),
                          child: const Text(
                            "Login",
                            style: TextStyle(
                              fontSize: 16,
                              color: Globals.color,
                            ),
                          ),
                        ),
                      ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
