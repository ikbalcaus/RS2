import "dart:convert";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:admin/providers/auth_provider.dart";
import "package:admin/screens/master_screen.dart";
import "books_screen.dart";

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final _formKey = GlobalKey<FormState>();
  bool _isLoading = false;
  Map<String, List<String>> _fieldErrors = {};

  void _login() async {
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
      setState(() {
        _fieldErrors = {
          "general": ["Only admin and moderator have access to the dashboard"],
        };
      });
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
      } catch (e) {
        setState(() {
          _fieldErrors = {
            "general": ["Unknown error occurred."],
          };
        });
      }
    }

    setState(() {
      _isLoading = false;
    });
  }

  Widget _buildErrorMessages(String fieldName) {
    if (!_fieldErrors.containsKey(fieldName)) return const SizedBox.shrink();
    final errors = _fieldErrors[fieldName]!;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: errors.map((msg) {
        return Padding(
          padding: const EdgeInsets.only(top: 2.0),
          child: Text(
            msg,
            style: const TextStyle(
              color: Colors.red,
              fontSize: 12,
              height: 1.4,
            ),
          ),
        );
      }).toList(),
    );
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
                Container(
                  width: double.infinity,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      TextFormField(
                        controller: _emailController,
                        decoration: const InputDecoration(
                          labelText: "Email",
                          border: OutlineInputBorder(),
                          isDense: true,
                          contentPadding: EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 14,
                          ),
                        ),
                        keyboardType: TextInputType.emailAddress,
                      ),
                      const SizedBox(height: 4),
                      _buildErrorMessages("email"),
                    ],
                  ),
                ),
                const SizedBox(height: 16.0),
                Container(
                  width: double.infinity,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      TextFormField(
                        controller: _passwordController,
                        decoration: const InputDecoration(
                          labelText: "Password",
                          border: OutlineInputBorder(),
                          isDense: true,
                          contentPadding: EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 14,
                          ),
                        ),
                        obscureText: true,
                      ),
                      const SizedBox(height: 4),
                      _buildErrorMessages("password"),
                    ],
                  ),
                ),
                Align(
                  alignment: Alignment.centerLeft,
                  child: _buildErrorMessages("general"),
                ),
                const SizedBox(height: 16.0),
                _isLoading
                    ? const CircularProgressIndicator()
                    : SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: _login,
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.blue,
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.zero,
                            ),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                          ),
                          child: const Text(
                            "Login",
                            style: TextStyle(
                              fontSize: 16,
                              color: Colors.white,
                              fontWeight: FontWeight.bold,
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
