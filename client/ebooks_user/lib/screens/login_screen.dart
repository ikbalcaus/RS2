import "dart:convert";
import "dart:io";
import "package:ebooks_user/screens/forgot_password_screen.dart";
import "package:ebooks_user/screens/profile_screen.dart";
import "package:ebooks_user/screens/register_screen.dart";
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
      _fieldErrors.clear();
    });
    final authProvider = Provider.of<AuthProvider>(context, listen: false);
    try {
      final response = await authProvider.login(
        _emailController.text,
        _passwordController.text,
      );
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
              return MapEntry(key, messages);
            });
          });
        } catch (ex) {
          setState(() {
            _fieldErrors = {
              "general": ["An error occurred. Please try again"],
            };
          });
        }
      }
    } on SocketException {
      setState(() {
        _fieldErrors = {
          "general": ["No internet connection"],
        };
      });
    } catch (ex) {
      setState(() {
        _fieldErrors = {
          "general": ["An error occurred. Please try again"],
        };
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;
    return Center(
      child: SingleChildScrollView(
        padding: const EdgeInsets.symmetric(horizontal: 24),
        child: Container(
          padding: const EdgeInsets.all(32),
          decoration: BoxDecoration(
            color: isDarkMode
                ? const Color.fromARGB(255, 24, 22, 32)
                : Colors.white,
            borderRadius: BorderRadius.circular(4),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withOpacity(0.05),
                blurRadius: 12,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          width: 400,
          child: Form(
            key: _formKey,
            child: Column(
              children: [
                Text(
                  "Login",
                  style: TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.w700,
                    color: isDarkMode ? Colors.white : Colors.black,
                  ),
                ),
                const SizedBox(height: 12),
                TextFormField(
                  controller: _emailController,
                  decoration: InputDecoration(
                    prefixIcon: const Icon(Icons.email_outlined),
                    labelText: "Email",
                    errorText: _fieldErrors["Email"]?.first,
                  ),
                  keyboardType: TextInputType.emailAddress,
                ),
                const SizedBox(height: 8),
                TextFormField(
                  controller: _passwordController,
                  decoration: InputDecoration(
                    prefixIcon: const Icon(Icons.lock_outline),
                    labelText: "Password",
                    errorText: _fieldErrors["Password"]?.first,
                  ),
                  obscureText: true,
                ),
                if (_fieldErrors.containsKey("general")) ...[
                  const SizedBox(height: 8),
                  Align(
                    alignment: Alignment.centerLeft,
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: _fieldErrors["general"]!
                          .map(
                            (msg) => Padding(
                              padding: const EdgeInsets.only(top: 4.0),
                              child: Text(
                                msg,
                                style: const TextStyle(
                                  color: Colors.red,
                                  fontSize: 13,
                                ),
                              ),
                            ),
                          )
                          .toList(),
                    ),
                  ),
                ],
                const SizedBox(height: 24),
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton(
                    onPressed: () async => await _login(),
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Globals.backgroundColor,
                      padding: const EdgeInsets.symmetric(vertical: 16),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(2),
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
                const SizedBox(height: 16),
                Column(
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Text(
                          "Don't have an account?",
                          style: TextStyle(
                            color: isDarkMode ? Colors.white70 : Colors.black87,
                            fontSize: 14,
                          ),
                        ),
                        TextButton(
                          onPressed: () => Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) => const RegisterScreen(),
                            ),
                          ),
                          child: const Text("Sign up"),
                        ),
                      ],
                    ),
                    TextButton(
                      onPressed: () {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (_) => const ForgotPasswordScreen(),
                          ),
                        );
                      },
                      child: Text("Forgot Password?"),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
