<?php
require 'ConnectionSettings.php';

$id = $_POST["id"];

if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

$sql = "SELECT Custom_json FROM usersData WHERE id = '". $id . "'";
$result = $conn->query($sql);

if ($result->num_rows > 0) {
  $row = $result->fetch_assoc();
  echo ($row["Custom_json"]);
} else {
  echo ("error, No data found for the given id");
}

$conn->close();
?>
