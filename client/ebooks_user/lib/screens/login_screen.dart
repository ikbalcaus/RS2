import "dart:convert";
import "package:ebooks_user/screens/profile_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:ebooks_user/providers/auth_provider.dart";

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

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

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
        final Map<String, dynamic> data = jsonDecode(response.body);
        AuthProvider.userId = data["userId"];
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => const ProfileScreen()),
        );
      }
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
    return Center(
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
                          style: TextStyle(fontSize: 16, color: Colors.white),
                        ),
                      ),
                    ),
            ],
          ),
        ),
      ),
    );
  }
}
