Feature: DataTables

  Scenario: 数据表内含注释、有空行
    Given a data table with comments and newlines inside
      | name  | sex    | age  |

      | sky   | male   |  18  |
      # this is a comment
      | fly   | female |  16  |