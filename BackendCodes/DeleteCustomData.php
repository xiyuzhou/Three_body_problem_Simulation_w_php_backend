<?php
require 'ConnectionSettings.php';

$id = $_POST["id"];

if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

$sql = "DELETE FROM usersData WHERE id = '". $id . "'";

if ($conn->query($sql) === TRUE) {
  echo "Item deleted successfully";
} else {
  echo "Error deleting item: " . $conn->error;
}

$conn->close();
?>
