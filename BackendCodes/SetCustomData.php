<?php
require 'ConnectionSettings.php';

$userID = $_POST["userID"];
$custom_data_name = $_POST["Data_name"];
$custom_json_data = $_POST["custom_json"];

if ($conn->connect_error) {
  die("Connection failed: " . $conn->connect_error);
}

// Check if the data name already exists for the given user
$check_sql = "SELECT * FROM `usersData` WHERE `dataset_name` = '$custom_data_name' AND `userID` = '$userID'";
$check_result = $conn->query($check_sql);

if ($check_result->num_rows > 0) {
  // If a record with the same data name and user ID exists, echo an error message
  echo "Name exists, please use a different one";
} else {
  // If the data name doesn't exist for the given user, proceed with insertion
  $insert_sql = "INSERT INTO `usersData`(`dataset_name`, `userID`, `Custom_json`) VALUES ('$custom_data_name', '$userID', '$custom_json_data')";
  
  if ($conn->query($insert_sql) === TRUE) {
    echo "New record created successfully";
  } else {
    echo "Error: " . $insert_sql . "<br>" . $conn->error;
  }
}

$conn->close();
?>
