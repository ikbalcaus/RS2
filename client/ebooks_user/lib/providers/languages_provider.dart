import "package:ebooks_user/models/languages/language.dart";
import "package:ebooks_user/providers/base_provider.dart";

class LanguagesProvider extends BaseProvider<Language> {
  LanguagesProvider() : super("languages");

  @override
  Language fromJson(data) {
    return Language.fromJson(data);
  }
}
