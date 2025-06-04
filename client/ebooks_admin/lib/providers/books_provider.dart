import "package:ebooks_admin/models/books/book.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/constants.dart";
import "package:http/http.dart" as http;

class BooksProvider extends BaseProvider<Book> {
  BooksProvider() : super("books");

  @override
  Book fromJson(data) {
    return Book.fromJson(data);
  }

  Future adminDelete(int id, String? reason) async {
    var uri = Uri.parse("${Constants.apiAddress}/books/$id/admin-delete");
    if (reason != null && reason.isNotEmpty) {
      uri = uri.replace(queryParameters: {"reason": reason});
    }
    var headers = createHeaders();
    var response = await http.delete(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
