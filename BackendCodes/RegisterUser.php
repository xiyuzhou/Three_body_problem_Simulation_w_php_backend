<?php
require 'ConnectionSettings.php';
//variables by users
$loginUser = $_POST["loginUser"];
$loginPass = $_POST["loginPass"];

// Create connection


// Check connection
if ($conn->connect_error) {
  $response["success"] = false;
  $response["message"] = "Connection failed";
  die("Connection failed: " . $conn->connect_error);
}

$response = array();

$sql = "SELECT username FROM user WHERE username = '". $loginUser . "'";

$result = $conn->query($sql);

if ($result->num_rows > 0) {
    $response["success"] = false;
    $response["message"] = "Username already taken";
}
else
{
    $sql2 = "INSERT INTO user (username,password) VAlUES ( '" . $loginUser . "', '".$loginPass ."')";
    if ($conn->query($sql2) === TRUE){
        $response["success"] = true;
        $response["message"] = "Register successful";
    }
    else{
        $response["success"] = false;
        $response["message"] = "Error: " . $sql2 . "<br>" . $conn->error;
    }
}

$conn->close();
echo json_encode($response);
?>