import "package:ebooks_user/models/access_rights/access_right.dart";
import "package:ebooks_user/providers/base_provider.dart";
import "package:ebooks_user/utils/globals.dart";
import "package:http/http.dart" as http;

class AccessRightsProvider extends BaseProvider<AccessRight> {
  AccessRightsProvider() : super("accessrights");

  @override
  AccessRight fromJson(data) {
    return AccessRight.fromJson(data);
  }

  Future toggleFavorite(int id) async {
    var uri = Uri.parse("${Globals.apiAddress}/accessrights/$id/favorite");
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (!isValidResponse(response)) {
      throw response.body;
    }
  }
}
