function toggle() {
    let input_toggle = document.getElementById("toggle_button_eye");
    let password_input = document.getElementById("custom-password-input");
  
    if (password_input.type === "password") {
      password_input.type = "text";
      input_toggle.src = "/images/icons/visibility.svg";
    } else {
      password_input.type = "password";
      input_toggle.src = "/images/icons/visibility_off.svg";
    }
  }