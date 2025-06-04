import "package:ebooks_admin/models/genres/genre.dart";
import "package:ebooks_admin/providers/base_provider.dart";

class GenresProvider extends BaseProvider<Genre> {
  GenresProvider() : super("genres");

  @override
  Genre fromJson(data) {
    return Genre.fromJson(data);
  }
}
