#!/bin/bash
# Loops through a list of websites and checks if each is reachable using curl

websites=(
  "https://www.google.com"
  "https://www.github.com"
  "https://www.microsoft.com"
  "https://thissitedoesnotexist12345.com"
)

for site in "${websites[@]}"; do
  if curl -s --head --request GET "$site" --max-time 5 | grep "200\|301\|302" > /dev/null; then
    echo "$site is REACHABLE"
  else
    echo "$site is NOT REACHABLE"
  fi
done