import "package:ebooks_admin/models/languages/language.dart";
import "package:ebooks_admin/providers/base_provider.dart";

class LanguagesProvider extends BaseProvider<Language> {
  LanguagesProvider() : super("languages");

  @override
  Language fromJson(data) {
    return Language.fromJson(data);
  }
}
