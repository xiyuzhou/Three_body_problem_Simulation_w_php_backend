<?php

require 'ConnectionSettings.php';
//variables by users
$loginUser = $_POST["loginUser"];
$loginPass = $_POST["loginPass"];

$response = array();
// Check connection
if ($conn->connect_error) {
  $response["success"] = false;
  $response["message"] = "Connection failed";
  die("Connection failed: " . $conn->connect_error);
}

$sql = "SELECT password, id FROM user WHERE username = '". $loginUser . "'";

$result = $conn->query($sql);

if ($result->num_rows > 0) {
  // output data of each row
  while($row = $result->fetch_assoc()) {
    if($row["password"] == $loginPass){

        $response["success"] = true;
        $response["message"] = "Login successful";
        $response["UserID"] = $row["id"];
    }
    else{
        $response["success"] = false;
        $response["message"] = "Incorrect password";
    }
  }
} else {
  $response["success"] = false;
  $response["message"] = "Username does not exist";
}
$conn->close();

echo json_encode($response);
?>