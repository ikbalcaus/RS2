import "package:ebooks_user/models/genres/genre.dart";
import "package:ebooks_user/providers/base_provider.dart";

class GenresProvider extends BaseProvider<Genre> {
  GenresProvider() : super("genres");

  @override
  Genre fromJson(data) {
    return Genre.fromJson(data);
  }
}
