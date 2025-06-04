import "dart:convert";
import "package:ebooks_admin/models/roles/role.dart";
import "package:ebooks_admin/providers/base_provider.dart";
import "package:ebooks_admin/utils/constants.dart";
import "package:http/http.dart" as http;

class RolesProvider extends BaseProvider<Role> {
  RolesProvider() : super("roles");

  @override
  Role fromJson(data) {
    return Role.fromJson(data);
  }

  Future assignRole(int userId, int roleId) async {
    var uri = Uri.parse(
      "${Constants.apiAddress}/roles/$userId/assign-role/$roleId",
    );
    var headers = createHeaders();
    var response = await http.patch(uri, headers: headers);
    if (isValidResponse(response)) {
      var data = jsonDecode(response.body);
      return fromJson(data);
    } else {
      throw response.body;
    }
  }
}
