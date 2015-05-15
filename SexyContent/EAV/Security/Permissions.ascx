<div sxc-app="PermissionsApp" eav-app="PermissionsApp" xid="app-[Module:ModuleId]" xclass="ng-cloak">
    <div ng-controller="Admin as vm">
        <a ng-href="{{vm.getUrl('new')}}" target="_self" class="btn btn-default">
            <span class="glyphicon glyphicon-plus"></span> New
        </a>
        <button type="button" class="btn btn-default" ng-click="refresh()">
            <span class="glyphicon glyphicon-repeat"></span> Refresh
        </button>
        <a target="_self" class="btn btn-default" ng-if="returnUrl" ng-href="{{returnUrl}}">
            <span class="glyphicon glyphicon-arrow-left"></span> Back
        </a>

        <table class="table table-striped table-hover">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>ID</th>
                    <th>Condition</th>
                    <th>Grant</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <tr ng-repeat="perm in vm.permissions | orderBy:'Title'">
                    <td><a ng-href="{{ vm.getUrl('edit', perm.Id) }}" target="_self">{{perm.Title}}</a></td>
                    <td>{{perm.Id}}</td>
                    <td>{{perm.Condition}}</td>
                    <td>{{perm.Grant}}</td>
                    <td>
                        <button type="button" class="btn btn-xs btn-danger" ng-click="vm.tryToDelete(perm.Title, perm.Id)">
                            <span class="glyphicon glyphicon-remove"></span> Delete
                        </button>
                    </td>
                </tr>
                <tr ng-if="!vm.permissions.length">
                    <td colspan="100">No Items</td>
                </tr>
            </tbody>
        </table>

    </div>
</div>
