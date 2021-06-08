let bookName, authorName, isbn;

var updatingBookId;

var globalBookId = 2;
let isUpdating = false;

/**
 * Book example : - 
 * {
 *      id : {
 *            bookName: 'abc',
 *            authorName: 'xyz',
 *            isbn: '1212'
 *      }
 * }
 * */

let booksArr = {};

async function setGlobalValues(flag) {
    bookName = $("#bookName").val();
    authorName = $("#authorName").val();
    isbn = $("#isbn").val();

    if (flag) {
        updatingBookId = Number($("#bookId").val());
        console.log('fuck u');
    }
    else {

    }
}

async function validateForm() {
    if (bookName !== undefined && authorName !== undefined && isbn !== undefined && globalBookId !== undefined) {
        if (bookName !== '' && authorName !== '' && isbn !== '') {

            //isUpdating = globalBookId === 0 ? false : true;

            return true;
        }
    }
    return false;
}

async function addRow() {
    let table = document.getElementById("myBooks");

    let newBook = document.createElement("tr");
    newBook.setAttribute("id", `bookId_${globalBookId}`);
    $(newBook).append(`<td>${globalBookId}</td>`);
    $(newBook).append(`<td>${bookName}</td>`);
    $(newBook).append(`<td>${authorName}</td>`);
    $(newBook).append(`<td>${isbn}</td>`);
    $(newBook).append(`<td>
                    <i onclick="UpdateBook(${globalBookId},'${bookName}','${authorName}','${isbn}')" class="fas fa-edit"></i>
                    <i onclick="deleteBook(${globalBookId})" class="trash fas fa-trash-alt"></i>
                </td>`);


    table.append(newBook);

    let bookToAdd = {
        bookName: bookName,
        authorName: authorName,
        isbn: isbn
    };

    booksArr[`id_${globalBookId}`] = bookToAdd;

    globalBookId++;
}



async function UpdateBook(bookId, bookName, authorName, isbn) {
    isUpdating = true;

    $("#cancelUpdate").show();

    console.log('update');

    $("#upsertBtn").val("UPDATE");

    $("#bookId").val(bookId);
    $("#bookName").val(bookName);
    $("#authorName").val(authorName);
    $("#isbn").val(isbn);
}

async function fillBookToUpdate() {

}

async function clearForm() {
    //globalBookId = 0;
    bookName = undefined;
    authorName = undefined;
    isbn = undefined;

    $("#bookId").val('');
    $("#bookName").val('');
    $("#authorName").val('');
    $("#isbn").val('');
}

async function deleteBook(bookIdToDelete) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $(`#bookId_${bookIdToDelete}`).remove();

            delete booksArr[`id_${bookIdToDelete}`];

            Swal.fire(
                'Deleted!',
                'Book has been deleted.',
                'success'
            )
            //if (document.querySelector("#myBooks tbody").children.length === 0) {
            //    let emptyDiv = document.createElement("div")
            //    emptyDiv.style.background = "lightgrey";
            //    emptyDiv.innerText = "No Data Available.";
            //    emptyDiv.style.width = "100%";

            //    document.querySelector("#myBooks tbody").append(emptyDiv);
            //}
        }
    })

    //showing no rows..
    
    
}

$().ready(() => {
    let firstBook = {
        bookName: 'Harry potter 1',
        authorName: 'JK Rowling',
        isbn: '12841'
    };

    booksArr['id_1'] = firstBook;

    $("#cancelUpdate").click(e => {

        clearForm();

        $("#cancelUpdate").hide();

        isUpdating = false;
        $("#upsertBtn").val('ADD');

    });

    $("#upsertBtn").click((e) => {

        if (isUpdating) {
            setGlobalValues(true);
        }
        else {
            setGlobalValues(false);
        }
        validateForm().then((resp) => {
            //successfully validated..
            if (resp) {
                if (!isUpdating) {
                    addRow();
                }
                //Updating..
                else {
                    let rowToUpdate = document.getElementById(`bookId_${updatingBookId}`);

                    //0  -  1    -  2    -  3
                    //id - bname - aname - isbn

                    rowToUpdate.children[1].innerText = bookName;
                    rowToUpdate.children[2].innerText = authorName;
                    rowToUpdate.children[3].innerText = isbn;

                    $(rowToUpdate).fadeOut(1000).fadeIn(500);

                    booksArr[`id_${updatingBookId}`] = {
                        bookName: bookName,
                        authorName: authorName,
                        isbn: isbn
                    }

                    isUpdating = false;
                    $("#upsertBtn").val("ADD");
                    $("#cancelUpdate").hide();
                    clearForm();
                }
            }
            //Validation is negative
            else {
                toastr.error('All fields are mandatory.');
            }
        })

    });
})