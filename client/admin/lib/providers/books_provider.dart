import "package:admin/models/book.dart";
import "package:admin/providers/base_provider.dart";

class BooksProvider extends BaseProvider<Book> {
  BooksProvider() : super("Books");

  @override
  Book fromJson(data) {
    return Book.fromJson(data);
  }
}
