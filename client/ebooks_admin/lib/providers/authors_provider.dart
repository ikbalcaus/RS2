import "package:ebooks_admin/models/authors/author.dart";
import "package:ebooks_admin/providers/base_provider.dart";

class AuthorsProvider extends BaseProvider<Author> {
  AuthorsProvider() : super("authors");

  @override
  Author fromJson(data) {
    return Author.fromJson(data);
  }
}
