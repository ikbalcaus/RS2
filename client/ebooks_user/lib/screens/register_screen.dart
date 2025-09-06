import "dart:convert";
import "dart:io";
import "package:ebooks_user/providers/users_provider.dart";
import "package:ebooks_user/screens/master_screen.dart";
import "package:ebooks_user/screens/profile_screen.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:ebooks_user/utils/helpers.dart";
import "package:flutter/material.dart";
import "package:provider/provider.dart";
import "package:easy_localization/easy_localization.dart";

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  late UsersProvider _usersProvider;
  Map<String, List<String>> _fieldErrors = {};
  final TextEditingController _firstNameController = TextEditingController();
  final TextEditingController _lastNameController = TextEditingController();
  final TextEditingController _userNameController = TextEditingController();
  final TextEditingController _emailController = TextEditingController();
  final TextEditingController _passwordController = TextEditingController();
  final TextEditingController _confirmPasswordController =
      TextEditingController();
  final _formKey = GlobalKey<FormState>();

  @override
  void initState() {
    super.initState();
    _usersProvider = context.read<UsersProvider>();
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _emailController.dispose();
    _lastNameController.dispose();
    _userNameController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  Future _register() async {
    setState(() {
      _fieldErrors.clear();
    });
    if (_passwordController.text != _confirmPasswordController.text) {
      setState(() {
        _fieldErrors["ConfirmPassword"] = ["Lozinke se ne podudaraju".tr()];
      });
      return;
    }
    try {
      await _usersProvider.post({
        "firstName": _firstNameController.text,
        "lastName": _lastNameController.text,
        "userName": _userNameController.text,
        "email": _emailController.text,
        "password": _passwordController.text,
      }, null);
      if (mounted) {
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(builder: (context) => const ProfileScreen()),
        );
        Helpers.showSuccessMessage(context, "Nalog je uspješno kreiran".tr());
      }
    } on SocketException {
      setState(() {
        _fieldErrors = {
          "general": ["Nema internet konekcije".tr()],
        };
      });
    } catch (ex) {
      try {
        final Map<String, dynamic> data = jsonDecode(ex.toString());
        final errors = data["errors"] as Map<String, dynamic>;
        setState(() {
          _fieldErrors = errors.map((key, value) {
            final List<String> messages = List<String>.from(value);
            return MapEntry(key, messages.map((msg) => msg.tr()).toList());
          });
        });
      } catch (_) {
        setState(() {
          _fieldErrors = {
            "general": ["Došlo je do greške. Pokušajte ponovo".tr()],
          };
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isDarkMode = Theme.of(context).brightness == Brightness.dark;
    return MasterScreen(
      showBackButton: true,
      child: Center(
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
                    "Registracija".tr(),
                    style: TextStyle(
                      fontSize: 28,
                      fontWeight: FontWeight.w700,
                      color: isDarkMode ? Colors.white : Colors.black,
                    ),
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(
                        child: TextFormField(
                          controller: _firstNameController,
                          decoration: InputDecoration(
                            labelText: "Ime".tr(),
                            errorText: _fieldErrors["FirstName"]?.first,
                          ),
                        ),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: TextFormField(
                          controller: _lastNameController,
                          decoration: InputDecoration(
                            labelText: "Prezime".tr(),
                            errorText: _fieldErrors["LastName"]?.first,
                          ),
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _userNameController,
                    decoration: InputDecoration(
                      labelText: "Korisničko ime".tr(),
                      errorText: _fieldErrors["UserName"]?.first,
                    ),
                  ),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _emailController,
                    decoration: InputDecoration(
                      labelText: "Email".tr(),
                      errorText: _fieldErrors["Email"]?.first,
                    ),
                    keyboardType: TextInputType.emailAddress,
                  ),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _passwordController,
                    decoration: InputDecoration(
                      labelText: "Lozinka".tr(),
                      errorText: _fieldErrors["Password"]?.first,
                    ),
                    obscureText: true,
                  ),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _confirmPasswordController,
                    decoration: InputDecoration(
                      labelText: "Potvrdi lozinku".tr(),
                      errorText: _fieldErrors["ConfirmPassword"]?.first,
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
                      onPressed: () async => await _register(),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Globals.backgroundColor,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(2),
                        ),
                      ),
                      child: Text(
                        "Registruj se".tr(),
                        style: const TextStyle(
                          fontSize: 16,
                          color: Colors.white,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(height: 16),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Text(
                        "Već imate nalog?".tr(),
                        style: TextStyle(
                          color: isDarkMode ? Colors.white70 : Colors.black87,
                          fontSize: 14,
                        ),
                      ),
                      TextButton(
                        onPressed: () => Navigator.pushReplacement(
                          context,
                          MaterialPageRoute(
                            builder: (_) => const ProfileScreen(),
                          ),
                        ),
                        child: Text(
                          "Prijavi se".tr(),
                          style: const TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                      ),
                    ],
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
