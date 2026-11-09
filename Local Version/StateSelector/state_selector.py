"""Python code extracted from RAG_State_Selector_Text_Proto (2).ipynb.
Run setup and C# generators before this script. Original evaluation logic preserved.
"""


# ---- Original notebook cell 7 ----

# Flag for LLM
LLM_ONLY = False

# Behavior Profiles
test_behavior_profile_AggressiveMelee = True
test_behavior_profile_Doctor = True
test_behavior_profile_Guardian = True

# Scenario Chunks
test_scenario_1 = True
test_scenario_2 = True
test_scenario_3 = True


# ---- Original notebook cell 19 ----
import json

def Deserialize(json_path):
    # Loading JSON
    with open(json_path, "r") as file:
        deserializedObject = json.load(file)

    return deserializedObject


# ---- Original notebook cell 21 ----
from langchain_ollama import OllamaEmbeddings
from langchain_community.vectorstores import Chroma

# Setting up langchain vectore store....Should make for every AI Agent using framework?
embeddings = OllamaEmbeddings(model="mxbai-embed-large", keep_alive=7200)

def Build_Collection(collection_name, embedding_function):
    collection = Chroma(
        collection_name=collection_name,
        embedding_function=embeddings
    )

    return collection


# ---- Original notebook cell 22 ----

# Builds a collection from a JSON
def Build_Profile_Collection(behavior_profile):

    # Extracting Json profile
    collection_name = "scenario_" + behavior_profile["Name"].lower().replace(" ", "_")
    scenario_chunks = behavior_profile["ScenarioChunks"]

    # Creating Collection for Behavior Profile
    collection = Build_Collection(collection_name, embeddings)

    # Creating Unique IDs for each scenario chunk
    ids = []
    for i in range(len(scenario_chunks)):
        ids.append(str(i))

    # Creating metadata for each scenario chunk
    metadatas = []
    for i in range(len(scenario_chunks)):
        metadatas.append({
            "id": str(i),
            "profile_name": behavior_profile["Name"]
        })

    # Adding chunks to database
    collection.add_texts(
        texts=scenario_chunks,
        ids=ids,
        metadatas=metadatas
    )

    return collection


# ---- Original notebook cell 23 ----
# Building Collections for Each Behavior Profile from JSON

Aggressive_Melee = Deserialize("behavior_profile_Aggressive Melee.json")
Aggressive_Melee_Collection = Build_Profile_Collection(Aggressive_Melee)

Doctor = Deserialize("behavior_profile_Doctor.json")
Doctor_Collection = Build_Profile_Collection(Doctor)

Guardian = Deserialize("behavior_profile_Guardian.json")
Guardian_Collection = Build_Profile_Collection(Guardian)

print(Aggressive_Melee_Collection._collection.name)
print(Doctor_Collection._collection.name)
print(Guardian_Collection._collection.name)


# ---- Original notebook cell 25 ----
results = Aggressive_Melee_Collection.get()

# convert to normal
ids = results["ids"]
documents = results["documents"]

print("For Collection ",Aggressive_Melee_Collection._collection.name)
for i in range(len(ids)):
    print("ID:", ids[i])
    print("Chunk:", documents[i], "\n")


# ---- Original notebook cell 27 ----
Scenario_1 = Deserialize("game_context_1.json")
Scenario_2 = Deserialize("game_context_2.json")
Scenario_3 = Deserialize("game_context_3.json")
Scenario_4 = Deserialize("game_context_4.json")

Scenarios = [Scenario_1, Scenario_2, Scenario_3, Scenario_4]


# ---- Original notebook cell 29 ----
from ollama import Client

# Prepping LLM
alpaca = Client()


# ---- Original notebook cell 30 ----
import textwrap
import time


def extract_game_context_from(scenario):
    game_context = ""

    # Agent Context
    agent = scenario["Agent"]

    game_context += "AGENT CHARCTER:\n"

    for context in agent["Context"]:
        game_context += context + "\n"

    game_context += "\n"

    # Relevant Character Contexts
    game_context += "RELEVENT CHARACTERS: \n"
    for character in scenario["Characters"]:
        game_context += "Character: " + character["Name"] + "\n"

        for context in character["Context"]:
            game_context += context + "\n"

        game_context += "\n"

    return game_context



def extract_valid_states_from(scenario):
    valid_states_context = ""

    for state in scenario["ValidTransitionStates"]:
        state_name = state["Name"]
        state_description = state["Description"]

        valid_states_context += "State: " + state_name + "\n" + "Description: " + state_description + "\n\n"

    return valid_states_context



def select_state(scenario, behavior_profile, collection, model, temperature=0.1, debug=False):

    # Converting to single string
    game_context = extract_game_context_from(scenario)
    valid_states_context = extract_valid_states_from(scenario)


    # Retrieval
    retrieved_chunks = collection.similarity_search_with_relevance_scores(
        game_context,
        k=3,
        score_threshold=0.4
    )

    retrieved_scenario_chunks = ""

    for document, score in retrieved_chunks:
        retrieved_scenario_chunks += document.page_content + "\n"

        if debug:
            print("ID:", document.metadata["id"])
            print("Chunk:", document.page_content)
            print("Score:", score)

    prompt= ""

    # run without scenario chunks
    if LLM_ONLY:
        # Prompt
        prompt = (
        "Behavior profile of the Agent:\n"
        f"{behavior_profile["Description"]}\n\n"
        "Current runtime game context:\n"
        f"{game_context}\n\n"
        "With the above context select the best state for the agent to transition to.\n"
        "Make the most suitable decision from the perspective of the agent\n"
        "Only select a state from the below valid states.\n"
        "Select only ONE of the valid states.\n"
        "Do not select state/action that is impossible for the agent to perform.\n"
        "Reponse should be limited to only the selected state and the target, nothing else.\n"
        "If there are no valid states repond with INVALID\n\n"
        "Valid States:\n"
        f"{valid_states_context}"
        "\n"
        "Follow this response format:\n"
        "STATE: <state>\n"
        "TARGET: <target>\n"
        "If the selected state does not require a target respond with this:\n"
        "STATE: <state>\n"
        "TARGET: NONE\n"
        "If there are no valid states repond with:\n"
        "STATE: INVALID\n"
        "TARGET: NONE\n"
        )

    # run query normally
    else:
        # Prompt
        prompt = (
        "Behavior profile of the Agent:\n"
        f"{behavior_profile["Description"]}\n\n"
        "Current runtime game context:\n"
        f"{game_context}\n\n"
        "Scenario based behavioral context:\n"
        f"{retrieved_scenario_chunks}\n"
        "With the above context select the best state for the agent to transition to.\n"
        "Make the most suitable decision from the perspective of the agent\n"
        "Only select a state from the below valid states.\n"
        "Select only ONE of the valid states.\n"
        "Do not select state/action that is impossible for the agent to perform.\n"
        "Reponse should be limited to only the selected state and the target, nothing else.\n"
        "If there are no valid states repond with INVALID\n\n"
        "Valid States:\n"
        f"{valid_states_context}"
        "\n"
        "Follow this response format:\n"
        "STATE: <state>\n"
        "TARGET: <target>\n"
        "If the selected state does not require a target respond with this:\n"
        "STATE: <state>\n"
        "TARGET: NONE\n"
        "If there are no valid states repond with:\n"
        "STATE: INVALID\n"
        "TARGET: NONE\n"
        )

    if debug:
        print("_______________\n")
        print("Prompt:")
        print(prompt, "\n")

    # LLM
    start_time = time.perf_counter()

    response = alpaca.chat(
        model=model,
        messages=[ {"role": "user", "content": prompt}],
        options={ "temperature": temperature},
    )

    response_time = time.perf_counter() - start_time

    return response, response_time


# ---- Original notebook cell 32 ----
import csv

testResults = []

# Builds CSV Result file giving all statistics of every recorded query.
def Output_TestResultCSV():
    if LLM_ONLY is True:
        filename = "TestResults_LLM_ONLY.csv"
    else:
        filename = "TestResults.csv"

    # Build CSV
    with open(filename, mode="w", newline="", encoding="utf-8") as file:
        writer = csv.DictWriter(file, fieldnames=testResults[0].keys())

        writer.writeheader()
        writer.writerows(testResults)

    print("Results saved to ", filename)

# Builds a CSV Result file showing stats to compare the models.
def Output_ModelComparisonStats():
    if LLM_ONLY is True:
        filename = "ModelComparisons_LLM_ONLY.csv"
    else:
        filename = "ModelComparisons.csv"

    model_stats = {}
    model_stats_output = []

    # Collecting stats
    for result in testResults:
        if result["Model"] not in model_stats:
            model_stats[result["Model"]] = {
                "Model": result["Model"],
                "Behavior": result["Behavior"],
                "Recorded Count": 0,
                "Total Duration Sum": 0,
                "Load Duration Sum": 0,
                "Prompt Processing Sum": 0,
                "Token Generation Sum": 0,
                "Prompt Tokens Sum": 0,
                "Generated Tokens Sum": 0
            }

        # Collecting sum of each stat
        model_stats[result["Model"]]["Recorded Count"] += 1
        model_stats[result["Model"]]["Total Duration Sum"] += result["Total Duration"]
        model_stats[result["Model"]]["Load Duration Sum"] += result["Load Duration"]
        model_stats[result["Model"]]["Prompt Processing Sum"] += result["Prompt Processing"]
        model_stats[result["Model"]]["Token Generation Sum"] += result["Token Generation"]
        model_stats[result["Model"]]["Prompt Tokens Sum"] += result["Prompt Tokens"]
        model_stats[result["Model"]]["Generated Tokens Sum"] += result["Generated Tokens"]

    for stat in model_stats.values():
        average_stats = {
            "Model": stat["Model"],
            "Ave Total Duration": stat["Total Duration Sum"] / stat["Recorded Count"],
            "Ave Load Duration": stat["Load Duration Sum"] / stat["Recorded Count"],
            "Ave Prompt Processing": stat["Prompt Processing Sum"] / stat["Recorded Count"],
            "Ave Token Generation": stat["Token Generation Sum"] / stat["Recorded Count"]
        }

        model_stats_output.append(average_stats)

    # Build CSV
    with open(filename, mode="w", newline="", encoding="utf-8") as file:
        writer = csv.DictWriter(file, fieldnames=model_stats_output[0].keys())

        writer.writeheader()
        writer.writerows(model_stats_output)

    print("Results saved to ", filename)


# ---- Original notebook cell 35 ----


def Query_Scenario_List(scenario_list, behavior_profile, collection, model):
    i = 1
    for scenario in scenario_list:
        response, response_time = select_state(
            scenario,
            behavior_profile,
            collection,
            model=model,
            temperature=0.1,
            debug=False
        )

        # Saving for CSV Print
        result = {
          "Model": model,
          "Behavior": behavior_profile["Name"],
          "Scenario": i,
          "Response": response["message"]["content"],
          "Response Time": response_time,
          "Total Duration": response["total_duration"] / 1e9,
          "Load Duration": response["load_duration"] / 1e9,
          "Prompt Processing": response["prompt_eval_duration"] / 1e9,
          "Token Generation": response["eval_duration"] / 1e9,
          "Prompt Tokens": response.get("prompt_eval_count", 0),
          "Generated Tokens": response.get("eval_count", 0)
        }
        testResults.append(result)

        # Console test print
        print("--------------------------")
        print("Behavior: ", behavior_profile["Name"])
        print("Model: ", model)
        print("Response to Scenario ", i, ":")
        print(response["message"]["content"], "\n")
        print(f"Response Time: {response_time:.3f} seconds")
        print(f"Total Ollama Duration: {response['total_duration'] / 1e9:.3f}s")
        print(f"Model Load Duration: {response['load_duration'] / 1e9:.3f}s")
        print(f"Prompt Processing: {response['prompt_eval_duration'] / 1e9:.3f}s")
        print(f"Token Generation: {response['eval_duration'] / 1e9:.3f}s\n")
        print(f"Generated Tokens: {response["eval_count"]}")

        i = i + 1


# ---- Original notebook cell 37 ----
# Scenario 1, Including Prompt Printing
response, response_time = select_state(
    Scenario_1,
    Aggressive_Melee,
    Aggressive_Melee_Collection,
    model="phi4-mini",
    temperature=0.1,
    debug=True
)

print("Response to Scenario 1:")
print(response["message"]["content"], "\n")
print(f"Response Time: {response_time:.3f}s")


Query_Scenario_List(Scenarios, Aggressive_Melee, Aggressive_Melee_Collection, "phi4-mini")
Query_Scenario_List(Scenarios, Doctor, Doctor_Collection, "phi4-mini")
Query_Scenario_List(Scenarios, Guardian, Guardian_Collection, "phi4-mini")


# ---- Original notebook cell 40 ----
Query_Scenario_List(Scenarios, Aggressive_Melee, Aggressive_Melee_Collection, "gemma3:4b")
Query_Scenario_List(Scenarios, Doctor, Doctor_Collection, "gemma3:4b")
Query_Scenario_List(Scenarios, Guardian, Guardian_Collection, "gemma3:4b")


# ---- Original notebook cell 43 ----
Query_Scenario_List(Scenarios, Aggressive_Melee, Aggressive_Melee_Collection, "gemma3:12b")
Query_Scenario_List(Scenarios, Doctor, Doctor_Collection, "gemma3:12b")
Query_Scenario_List(Scenarios, Guardian, Guardian_Collection, "gemma3:12b")


# ---- Original notebook cell 46 ----
Query_Scenario_List(Scenarios, Aggressive_Melee, Aggressive_Melee_Collection, "qwen3:4b")
Query_Scenario_List(Scenarios, Doctor, Doctor_Collection, "qwen3:4b")
Query_Scenario_List(Scenarios, Guardian, Guardian_Collection, "qwen3:4b")


# ---- Original notebook cell 49 ----
Query_Scenario_List(Scenarios, Aggressive_Melee, Aggressive_Melee_Collection, "qwen3:8b")
Query_Scenario_List(Scenarios, Doctor, Doctor_Collection, "qwen3:8b")
Query_Scenario_List(Scenarios, Guardian, Guardian_Collection, "qwen3:8b")


# ---- Original notebook cell 52 ----
Output_TestResultCSV()
Output_ModelComparisonStats()
